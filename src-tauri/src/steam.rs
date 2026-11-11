//! Пошук іконки та банера гри в кеші бібліотеки Steam (порт `steamMedia.ts`).

use std::fs;
use std::path::{Path, PathBuf};

use regex::Regex;

pub struct SteamMedia {
    pub banner: Option<String>,
    pub icon: Option<String>,
}

fn normalize_fs_path(value: &str) -> String {
    value.replace('\\', "/").trim_end_matches('/').to_lowercase()
}

#[cfg(windows)]
fn query_registry_value(hive: &str, key: &str, value: &str) -> Option<String> {
    use winreg::enums::{HKEY_CURRENT_USER, HKEY_LOCAL_MACHINE};
    use winreg::RegKey;

    let root = if hive == "HKCU" { HKEY_CURRENT_USER } else { HKEY_LOCAL_MACHINE };
    RegKey::predef(root)
        .open_subkey(key)
        .ok()?
        .get_value::<String, _>(value)
        .ok()
}

#[cfg(windows)]
fn get_steam_install_path() -> Option<PathBuf> {
    let candidates = [
        ("HKCU", "Software\\Valve\\Steam", "SteamPath"),
        ("HKCU", "Software\\Valve\\Steam", "SteamExe"),
        ("HKLM", "SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath"),
        ("HKLM", "SOFTWARE\\Valve\\Steam", "InstallPath"),
    ];
    for (hive, key, value) in candidates {
        let Some(raw) = query_registry_value(hive, key, value) else {
            continue;
        };
        let cleaned = raw.replace('"', "");
        let cleaned = cleaned.trim();
        if cleaned.is_empty() {
            continue;
        }
        let resolved = if value == "SteamExe" {
            Path::new(cleaned).parent().map(Path::to_path_buf)
        } else {
            Some(PathBuf::from(cleaned))
        };
        if let Some(path) = resolved {
            return Some(path);
        }
    }
    None
}

#[cfg(not(windows))]
fn get_steam_install_path() -> Option<PathBuf> {
    None
}

fn get_steam_library_paths(steam_path: &Path) -> Vec<PathBuf> {
    let mut libraries = vec![steam_path.to_path_buf()];
    let library_file = steam_path.join("steamapps").join("libraryfolders.vdf");
    if let Ok(raw) = fs::read_to_string(&library_file) {
        let re = Regex::new(r#""path"\s+"([^"]+)""#).expect("valid regex");
        for caps in re.captures_iter(&raw) {
            let unescaped = caps[1].replace("\\\\", "\\");
            let candidate = PathBuf::from(unescaped);
            if !libraries.contains(&candidate) {
                libraries.push(candidate);
            }
        }
    }
    libraries
}

fn find_steam_app_id_by_path(game_path: &str, libraries: &[PathBuf]) -> Option<String> {
    let normalized_game = normalize_fs_path(game_path);
    let manifest_name = Regex::new(r"(?i)^appmanifest_\d+\.acf$").expect("valid regex");
    let appid_re = Regex::new(r#"(?i)"appid"\s+"(\d+)""#).expect("valid regex");
    let installdir_re = Regex::new(r#"(?i)"installdir"\s+"([^"]+)""#).expect("valid regex");

    for library in libraries {
        let steamapps = library.join("steamapps");
        let Ok(entries) = fs::read_dir(&steamapps) else {
            continue;
        };
        for entry in entries.flatten() {
            let name = entry.file_name().to_string_lossy().to_string();
            if !manifest_name.is_match(&name) {
                continue;
            }
            let Ok(content) = fs::read_to_string(entry.path()) else {
                continue;
            };
            let (Some(appid), Some(installdir)) = (
                appid_re.captures(&content).map(|c| c[1].to_string()),
                installdir_re.captures(&content).map(|c| c[1].to_string()),
            ) else {
                continue;
            };
            let install_path = normalize_fs_path(
                &steamapps.join("common").join(installdir).to_string_lossy(),
            );
            if normalized_game == install_path || normalized_game.starts_with(&format!("{install_path}/")) {
                return Some(appid);
            }
        }
    }
    None
}

fn pick_first_existing(base: &Path, candidates: &[&str]) -> Option<PathBuf> {
    candidates.iter().map(|c| base.join(c)).find(|p| p.exists())
}

fn pick_first_by_extensions(base: &Path, extensions: &[&str]) -> Option<PathBuf> {
    for entry in fs::read_dir(base).ok()?.flatten() {
        let path = entry.path();
        let ext = path
            .extension()
            .map(|e| format!(".{}", e.to_string_lossy().to_lowercase()))
            .unwrap_or_default();
        if extensions.contains(&ext.as_str()) {
            return Some(path);
        }
    }
    None
}

/// Блокуюча функція: викликати через `spawn_blocking`.
pub fn resolve_steam_media_for_path(game_path: &str) -> Option<SteamMedia> {
    let lowered = game_path.to_lowercase();
    if !lowered.contains("steamapps") && !lowered.contains("steamlibrary") {
        return None;
    }
    let steam_path = get_steam_install_path()?;
    let libraries = get_steam_library_paths(&steam_path);
    let app_id = find_steam_app_id_by_path(game_path, &libraries)?;

    let cache_dir = steam_path.join("appcache").join("librarycache").join(&app_id);
    if !cache_dir.exists() {
        return None;
    }

    let banner = pick_first_existing(
        &cache_dir,
        &["library_hero.jpg", "library_hero_blur.jpg", "header.jpg", "library_hero.png"],
    );
    let icon = pick_first_existing(
        &cache_dir,
        &[
            "library_600x900.jpg",
            "library_600x900_2x.jpg",
            "capsule_231x87.jpg",
            "capsule_231x87.png",
            "logo.png",
        ],
    );
    let fallback = pick_first_by_extensions(&cache_dir, &[".jpg", ".png"]);

    let to_string = |p: PathBuf| p.to_string_lossy().to_string();
    Some(SteamMedia {
        banner: banner.clone().or_else(|| fallback.clone()).map(to_string),
        icon: icon.or(fallback).map(to_string),
    })
}
