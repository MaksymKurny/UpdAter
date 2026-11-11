//! Завантаження файлів (зокрема з Google Drive) і встановлення перекладів (порт `download.ts`).

use std::path::{Path, PathBuf};
use std::process::Command;
use std::time::{Duration, Instant};

use futures_util::StreamExt;
use percent_encoding::percent_decode_str;
use regex::Regex;
use reqwest::header::{CONTENT_DISPOSITION, CONTENT_TYPE, COOKIE, LOCATION, SET_COOKIE};
use reqwest::redirect::Policy;
use reqwest::{Client, Response};
use tauri::path::BaseDirectory;
use tauri::{AppHandle, Emitter, Manager};
use tokio::io::AsyncWriteExt;

use crate::models::{DownloadProgress, DownloadResult};
use crate::store::Store;

const USER_AGENT: &str = concat!("UpdAter/", env!("CARGO_PKG_VERSION"));

const ARCHIVE_EXTENSIONS: &[&str] = &[
    ".zip", ".7z", ".rar", ".tar", ".tar.gz", ".tgz", ".tar.bz2", ".tbz", ".tbz2", ".tar.xz", ".txz",
    ".gz", ".bz2", ".xz", ".iso",
];

fn is_archive(path: &str) -> bool {
    let lower = path.to_lowercase();
    ARCHIVE_EXTENSIONS.iter().any(|ext| lower.ends_with(ext))
}

fn is_exe(path: &str) -> bool {
    path.to_lowercase().ends_with(".exe")
}

fn sanitize_file_name(name: &str) -> String {
    let re = Regex::new(r#"[<>:"/\\|?*]+"#).expect("valid regex");
    re.replace_all(name, "_").to_string()
}

// ---------------------------------------------------------------------------
// Google Drive
// ---------------------------------------------------------------------------

fn is_google_drive_url(url: &str) -> bool {
    Regex::new(r"drive\.google\.com|docs\.google\.com")
        .expect("valid regex")
        .is_match(url)
}

fn extract_drive_id(url: &str) -> Option<String> {
    let direct = Regex::new(r"/d/([a-zA-Z0-9_-]+)").expect("valid regex");
    if let Some(caps) = direct.captures(url) {
        return Some(caps[1].to_string());
    }
    reqwest::Url::parse(url)
        .ok()?
        .query_pairs()
        .find(|(key, _)| key == "id")
        .map(|(_, value)| value.to_string())
        .filter(|id| !id.is_empty())
}

fn normalize_drive_html(html: &str) -> String {
    html.replace("\\u003d", "=")
        .replace("\\u0026", "&")
        .replace("&amp;", "&")
}

fn parse_drive_confirm_token(html: &str) -> Option<String> {
    let normalized = normalize_drive_html(html);
    let patterns = [
        r"confirm=([0-9A-Za-z_-]+)",
        r#"name="confirm" value="([^"]+)""#,
        r#""confirm"\s*:\s*"([^"]+)""#,
    ];
    patterns.iter().find_map(|pattern| {
        Regex::new(pattern)
            .expect("valid regex")
            .captures(&normalized)
            .map(|caps| caps[1].to_string())
    })
}

fn extract_drive_download_url(html: &str) -> Option<String> {
    let normalized = normalize_drive_html(html);

    let direct_patterns = [
        r#"https?://drive\.usercontent\.google\.com/download\?[^"'\\\s>]+"#,
        r#"https?://drive\.google\.com/uc\?export=download[^"'\\\s>]+"#,
    ];
    for pattern in direct_patterns {
        if let Some(found) = Regex::new(pattern).expect("valid regex").find(&normalized) {
            return Some(found.as_str().to_string());
        }
    }

    let form_patterns = [
        r#"<form[^>]+id="download-form"[^>]+action="([^"]+)""#,
        r#"<form[^>]+action="([^"]+drive\.usercontent\.google\.com/download[^"]*)""#,
    ];
    let form_action = form_patterns.iter().find_map(|pattern| {
        Regex::new(pattern)
            .expect("valid regex")
            .captures(&normalized)
            .map(|caps| caps[1].to_string())
    })?;

    let input_re = Regex::new(r#"<input[^>]+name="([^"]+)"[^>]+value="([^"]*)""#).expect("valid regex");
    let mut serializer = reqwest::Url::parse("http://localhost/").expect("valid url");
    {
        let mut pairs = serializer.query_pairs_mut();
        for caps in input_re.captures_iter(&normalized) {
            pairs.append_pair(&caps[1], &caps[2]);
        }
    }
    match serializer.query() {
        Some(query) if !query.is_empty() => Some(format!("{form_action}?{query}")),
        _ => Some(form_action),
    }
}

fn is_drive_access_denied_html(html: &str) -> bool {
    let normalized = normalize_drive_html(html);
    Regex::new(r"(?i)accounts\.google\.com|ServiceLogin|request access|need access|access denied|permission")
        .expect("valid regex")
        .is_match(&normalized)
        || Regex::new(r"(?i)потрібен доступ|запит доступу|немає доступу")
            .expect("valid regex")
            .is_match(&normalized)
}

struct Resolved {
    url: String,
    cookie: Option<String>,
}

/// Збирає заголовок `Cookie` з усіх `Set-Cookie` відповіді.
fn cookie_header(response: &Response) -> Option<String> {
    let pairs: Vec<String> = response
        .headers()
        .get_all(SET_COOKIE)
        .iter()
        .filter_map(|value| value.to_str().ok())
        .filter_map(|value| value.split(';').next())
        .map(|pair| pair.trim().to_string())
        .filter(|pair| !pair.is_empty())
        .collect();
    if pairs.is_empty() {
        None
    } else {
        Some(pairs.join("; "))
    }
}

fn warning_cookie_token(response: &Response) -> Option<String> {
    let re = Regex::new(r"download_warning[^=]*=([^;]+)").expect("valid regex");
    response
        .headers()
        .get_all(SET_COOKIE)
        .iter()
        .filter_map(|value| value.to_str().ok())
        .find_map(|value| re.captures(value).map(|caps| caps[1].to_string()))
}

async fn resolve_google_drive(no_redirect: &Client, url: &str) -> Result<Resolved, String> {
    let Some(id) = extract_drive_id(url) else {
        return Ok(Resolved { url: url.to_string(), cookie: None });
    };
    let base = format!("https://drive.google.com/uc?export=download&id={id}");
    let initial = no_redirect.get(&base).send().await.map_err(|e| e.to_string())?;

    if let Some(location) = initial.headers().get(LOCATION).and_then(|v| v.to_str().ok()) {
        let resolved = reqwest::Url::parse(&base)
            .and_then(|b| b.join(location))
            .map(|u| u.to_string())
            .unwrap_or_else(|_| location.to_string());
        return Ok(Resolved { url: resolved, cookie: None });
    }

    let cookie = cookie_header(&initial);
    let cookie_confirm = warning_cookie_token(&initial);

    let content_type = initial
        .headers()
        .get(CONTENT_TYPE)
        .and_then(|v| v.to_str().ok())
        .unwrap_or("")
        .to_string();
    if !content_type.contains("text/html") {
        return Ok(Resolved { url: base, cookie });
    }

    let html = initial.text().await.map_err(|e| e.to_string())?;
    if let Some(direct) = extract_drive_download_url(&html) {
        return Ok(Resolved { url: direct, cookie });
    }

    match cookie_confirm.or_else(|| parse_drive_confirm_token(&html)) {
        Some(confirm) => Ok(Resolved { url: format!("{base}&confirm={confirm}"), cookie }),
        None => Ok(Resolved { url: base, cookie }),
    }
}

// ---------------------------------------------------------------------------
// Завантаження
// ---------------------------------------------------------------------------

fn file_name_from_response(response: &Response, url: &str) -> String {
    if let Some(disposition) = response.headers().get(CONTENT_DISPOSITION).and_then(|v| v.to_str().ok()) {
        let re = Regex::new(r#"(?i)filename\*?=(?:UTF-8'')?["']?([^"';]+)["']?"#).expect("valid regex");
        if let Some(caps) = re.captures(disposition) {
            return percent_decode_str(&caps[1]).decode_utf8_lossy().to_string();
        }
    }
    if let Ok(parsed) = reqwest::Url::parse(url) {
        if let Some(base) = parsed
            .path_segments()
            .and_then(|segments| segments.last())
            .filter(|segment| !segment.is_empty())
        {
            return percent_decode_str(base).decode_utf8_lossy().to_string();
        }
    }
    format!("download-{}", now_millis())
}

fn now_millis() -> u128 {
    std::time::SystemTime::now()
        .duration_since(std::time::UNIX_EPOCH)
        .map(|d| d.as_millis())
        .unwrap_or(0)
}

fn content_type_of(response: &Response) -> String {
    response
        .headers()
        .get(CONTENT_TYPE)
        .and_then(|v| v.to_str().ok())
        .unwrap_or("")
        .to_string()
}

async fn fetch(client: &Client, url: &str, cookie: &Option<String>) -> Result<Response, String> {
    let mut request = client.get(url);
    if let Some(cookie) = cookie {
        request = request.header(COOKIE, cookie);
    }
    let response = request.send().await.map_err(|e| format!("Не вдалося завантажити: {e}"))?;
    if !response.status().is_success() {
        return Err(format!(
            "Download failed: {} {}",
            response.status().as_u16(),
            response.status().canonical_reason().unwrap_or("")
        ));
    }
    Ok(response)
}

struct Downloaded {
    file_path: PathBuf,
    file_name: String,
}

async fn download_file(
    url: &str,
    dest_dir: &Path,
    mut on_progress: impl FnMut(u64, Option<u64>),
) -> Result<Downloaded, String> {
    let client = Client::builder().user_agent(USER_AGENT).build().map_err(|e| e.to_string())?;
    let no_redirect = Client::builder()
        .user_agent(USER_AGENT)
        .redirect(Policy::none())
        .build()
        .map_err(|e| e.to_string())?;

    let is_drive = is_google_drive_url(url);
    let resolved = if is_drive {
        resolve_google_drive(&no_redirect, url).await?
    } else {
        Resolved { url: url.to_string(), cookie: None }
    };

    let mut response = fetch(&client, &resolved.url, &resolved.cookie).await?;

    if is_drive && content_type_of(&response).contains("text/html") {
        let html = response.text().await.map_err(|e| e.to_string())?;
        if is_drive_access_denied_html(&html) {
            return Err("Google Drive: файл недоступний. Перевірте, чи відкрито доступ для всіх.".into());
        }

        let retry_url = extract_drive_download_url(&html)
            .filter(|retry| retry != &resolved.url)
            .or_else(|| {
                let confirm = parse_drive_confirm_token(&html)?;
                let id = extract_drive_id(url)?;
                Some(format!("https://drive.google.com/uc?export=download&id={id}&confirm={confirm}"))
            });

        let Some(retry_url) = retry_url else {
            return Err("Google Drive повернув HTML підтвердження. Перевірте доступ або посилання.".into());
        };
        response = fetch(&client, &retry_url, &resolved.cookie).await?;

        if content_type_of(&response).contains("text/html") {
            return Err("Google Drive повернув HTML підтвердження. Перевірте доступ або посилання.".into());
        }
    }

    let file_name = sanitize_file_name(&file_name_from_response(&response, &resolved.url));
    let file_path = dest_dir.join(format!("{}-{}", now_millis(), file_name));

    let total = response.content_length().filter(|len| *len > 0);
    let mut received: u64 = 0;
    let mut file = tokio::fs::File::create(&file_path)
        .await
        .map_err(|e| format!("Не вдалося створити файл: {e}"))?;

    let mut stream = response.bytes_stream();
    let mut last_emit = Instant::now() - Duration::from_secs(1);
    while let Some(chunk) = stream.next().await {
        let chunk = chunk.map_err(|e| format!("Помилка завантаження: {e}"))?;
        file.write_all(&chunk)
            .await
            .map_err(|e| format!("Не вдалося записати файл: {e}"))?;
        received += chunk.len() as u64;
        if last_emit.elapsed() >= Duration::from_millis(100) {
            last_emit = Instant::now();
            on_progress(received, total);
        }
    }
    file.flush().await.map_err(|e| e.to_string())?;
    on_progress(received, total);

    Ok(Downloaded { file_path, file_name })
}

// ---------------------------------------------------------------------------
// Розпаковка (7z)
// ---------------------------------------------------------------------------

fn find_seven_zip(app: &AppHandle) -> Result<PathBuf, String> {
    if cfg!(windows) {
        let mut candidates: Vec<PathBuf> = Vec::new();
        for relative in ["resources/7z/7z.exe", "7z/7z.exe", "7z.exe"] {
            if let Ok(path) = app.path().resolve(relative, BaseDirectory::Resource) {
                candidates.push(path);
            }
        }
        if let Some(dir) = std::env::current_exe().ok().and_then(|exe| exe.parent().map(Path::to_path_buf)) {
            candidates.push(dir.join("resources").join("7z").join("7z.exe"));
            candidates.push(dir.join("7z").join("7z.exe"));
        }
        return candidates
            .into_iter()
            .find(|path| path.exists())
            .ok_or_else(|| "Не знайдено 7z.exe серед ресурсів застосунку.".to_string());
    }

    // macOS / Linux: системний 7-Zip.
    for name in ["7zz", "7z", "7za"] {
        if Command::new(name).arg("--help").output().is_ok() {
            return Ok(PathBuf::from(name));
        }
    }
    Err("Не знайдено 7z. Встановіть p7zip / 7-Zip.".into())
}

fn extract_archive_blocking(seven_zip: PathBuf, archive: PathBuf, dest: PathBuf) -> Result<(), String> {
    std::fs::create_dir_all(&dest).map_err(|e| format!("Не вдалося створити теку {}: {e}", dest.display()))?;

    let dest_arg = dest.to_string_lossy().trim_end_matches(['\\', '/']).to_string();
    let mut command = Command::new(&seven_zip);
    command
        .arg("x")
        .arg("-y")
        .arg("-aoa")
        .arg("-bd")
        .arg(format!("-o{dest_arg}"))
        .arg(&archive);

    #[cfg(windows)]
    {
        use std::os::windows::process::CommandExt;
        command.creation_flags(0x0800_0000); // CREATE_NO_WINDOW
    }

    let output = command.output().map_err(|e| format!("Не вдалося запустити 7z: {e}"))?;
    // 0 - успіх, 1 - попередження (наприклад, файл зайнятий) - не фатально.
    match output.status.code() {
        Some(0) | Some(1) => Ok(()),
        _ => {
            let stderr = String::from_utf8_lossy(&output.stderr);
            let stdout = String::from_utf8_lossy(&output.stdout);
            let details = if stderr.trim().is_empty() { stdout } else { stderr };
            Err(format!("Не вдалося розпакувати архів: {}", details.trim()))
        }
    }
}

// ---------------------------------------------------------------------------
// Основний сценарій
// ---------------------------------------------------------------------------

pub async fn download_and_install(app: AppHandle, store: &Store, id: &str) -> Result<DownloadResult, String> {
    let item = store.find(id).ok_or("Переклад не знайдено.")?;
    if item.path.trim().is_empty() {
        return Err("Не вказано шлях встановлення.".into());
    }

    let download_dir = store.dir().join("downloads").join(id);
    tokio::fs::create_dir_all(&download_dir)
        .await
        .map_err(|e| format!("Не вдалося створити теку завантажень: {e}"))?;

    let progress_app = app.clone();
    let progress_id = id.to_string();
    let downloaded = download_file(&item.url, &download_dir, move |received, total| {
        let percent = total.map(|t| ((received as f64 / t as f64) * 100.0).round().min(100.0) as u8);
        let _ = progress_app.emit(
            "download-progress",
            DownloadProgress { id: progress_id.clone(), received, total, percent },
        );
    })
    .await?;

    let file_path_str = downloaded.file_path.to_string_lossy().to_string();
    let exe = is_exe(&file_path_str);
    let mut extracted = false;
    let mut copied = false;

    if is_archive(&file_path_str) {
        let seven_zip = find_seven_zip(&app)?;
        let archive = downloaded.file_path.clone();
        let dest = PathBuf::from(&item.path);
        tokio::task::spawn_blocking(move || extract_archive_blocking(seven_zip, archive, dest))
            .await
            .map_err(|e| e.to_string())??;
        extracted = true;
    } else if !exe {
        let dest_dir = PathBuf::from(&item.path);
        tokio::fs::create_dir_all(&dest_dir)
            .await
            .map_err(|e| format!("Не вдалося створити теку {}: {e}", dest_dir.display()))?;
        tokio::fs::copy(&downloaded.file_path, dest_dir.join(&downloaded.file_name))
            .await
            .map_err(|e| format!("Не вдалося скопіювати файл: {e}"))?;
        copied = true;
    }

    let mut patch = serde_json::Map::new();
    patch.insert(
        "LastUpdate".into(),
        serde_json::Value::String(chrono::Utc::now().to_rfc3339_opts(chrono::SecondsFormat::Millis, true)),
    );
    store.update(id, patch)?;

    let result = DownloadResult {
        id: id.to_string(),
        file_path: file_path_str,
        file_name: downloaded.file_name,
        is_exe: exe,
        extracted,
        copied,
    };
    let _ = app.emit("download-complete", result.clone());
    Ok(result)
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::io::{Read, Write};
    use std::net::TcpListener;

    #[test]
    fn extracts_drive_id_from_both_url_forms() {
        assert_eq!(
            extract_drive_id("https://drive.google.com/file/d/1AbC-d_E/view?usp=sharing").as_deref(),
            Some("1AbC-d_E")
        );
        assert_eq!(
            extract_drive_id("https://drive.google.com/uc?export=download&id=XYZ123").as_deref(),
            Some("XYZ123")
        );
        assert_eq!(extract_drive_id("https://example.com/file.zip"), None);
    }

    #[test]
    fn parses_drive_confirm_form() {
        let html = r#"<form id="download-form" action="https://drive.usercontent.google.com/download" method="get">
            <input type="hidden" name="id" value="FILEID"><input type="hidden" name="export" value="download">
            <input type="hidden" name="confirm" value="t"><input type="hidden" name="uuid" value="abc-123"></form>"#;
        let url = extract_drive_download_url(html).expect("form url");
        assert!(url.starts_with("https://drive.usercontent.google.com/download?"));
        assert!(url.contains("id=FILEID") && url.contains("confirm=t") && url.contains("uuid=abc-123"));
        assert_eq!(parse_drive_confirm_token(html).as_deref(), Some("t"));
    }

    #[test]
    fn direct_drive_url_keeps_letter_s() {
        // У JS-версії клас символів `[^"'\\s>]` помилково виключав літеру "s".
        let html = r#"href="https://drive.usercontent.google.com/download?id=1&export=download&authuser=0&confirm=t""#;
        let url = extract_drive_download_url(html).expect("direct url");
        assert!(url.ends_with("authuser=0&confirm=t"), "{url}");
    }

    #[test]
    fn detects_archives_and_exe() {
        assert!(is_archive("C:/x/Game_UA.ZIP"));
        assert!(is_archive("pack.tar.gz"));
        assert!(!is_archive("patch.exe"));
        assert!(is_exe("Setup.EXE"));
        assert_eq!(sanitize_file_name("a:b/c?.zip"), "a_b_c_.zip");
    }

    #[tokio::test]
    async fn downloads_file_with_progress_and_disposition_name() {
        let body: Vec<u8> = (0..200_000u32).map(|i| (i % 251) as u8).collect();
        let listener = TcpListener::bind("127.0.0.1:0").unwrap();
        let port = listener.local_addr().unwrap().port();
        let served = body.clone();
        std::thread::spawn(move || {
            let (mut stream, _) = listener.accept().unwrap();
            let mut buf = [0u8; 2048];
            let _ = stream.read(&mut buf);
            let head = format!(
                "HTTP/1.1 200 OK\r\nContent-Length: {}\r\nContent-Disposition: attachment; filename*=UTF-8''%D0%9F%D0%B0%D1%82%D1%87.bin\r\nConnection: close\r\n\r\n",
                served.len()
            );
            stream.write_all(head.as_bytes()).unwrap();
            stream.write_all(&served).unwrap();
        });

        let dir = std::env::temp_dir().join(format!("updater-test-{}", now_millis()));
        std::fs::create_dir_all(&dir).unwrap();
        let mut last = (0u64, None);
        let result = download_file(&format!("http://127.0.0.1:{port}/x"), &dir, |r, t| last = (r, t))
            .await
            .expect("download");

        assert_eq!(result.file_name, "Патч.bin");
        assert_eq!(std::fs::read(&result.file_path).unwrap(), body);
        assert_eq!(last, (body.len() as u64, Some(body.len() as u64)));
        let _ = std::fs::remove_dir_all(&dir);
    }
}
