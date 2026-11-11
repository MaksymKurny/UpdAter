use serde_json::{Map, Value};
use tauri::{AppHandle, State};
use tauri_plugin_opener::OpenerExt;

use crate::download;
use crate::models::{DownloadResult, TranslationEntry, TranslationInput};
use crate::steam;
use crate::store::Store;

fn non_empty(value: Option<String>) -> Option<String> {
    value.map(|v| v.trim().to_string()).filter(|v| !v.is_empty())
}

#[tauri::command]
pub fn get_translations(store: State<'_, Store>) -> Vec<TranslationEntry> {
    store.get_all()
}

#[tauri::command]
pub async fn add_translation(
    store: State<'_, Store>,
    input: TranslationInput,
) -> Result<TranslationEntry, String> {
    let mut entry = TranslationEntry {
        id: uuid::Uuid::new_v4().to_string(),
        title: input.title.trim().to_string(),
        url: input.url.trim().to_string(),
        guide_url: Some(input.guide_url.unwrap_or_default().trim().to_string()),
        path: input.path.trim().to_string(),
        icon: non_empty(input.icon),
        banner: non_empty(input.banner),
        last_update: None,
        add_to_list: input.add_to_list,
        pinned_state: input.pinned_state,
        meta_info: non_empty(input.meta_info),
    };

    if entry.icon.is_none() || entry.banner.is_none() {
        let path = entry.path.clone();
        let media = tokio::task::spawn_blocking(move || steam::resolve_steam_media_for_path(&path))
            .await
            .map_err(|e| e.to_string())?;
        if let Some(media) = media {
            if entry.icon.is_none() {
                entry.icon = media.icon;
            }
            if entry.banner.is_none() {
                entry.banner = media.banner;
            }
        }
    }

    store.add(entry)
}

#[tauri::command]
pub fn update_translation(
    store: State<'_, Store>,
    id: String,
    patch: Map<String, Value>,
) -> Result<Option<TranslationEntry>, String> {
    store.update(&id, patch)
}

#[tauri::command]
pub fn remove_translation(store: State<'_, Store>, id: String) -> Result<bool, String> {
    store.remove(&id)
}

#[tauri::command]
pub fn toggle_favorite(store: State<'_, Store>, id: String) -> Result<Option<TranslationEntry>, String> {
    store.toggle_favorite(&id)
}

#[tauri::command]
pub async fn download_and_install(
    app: AppHandle,
    store: State<'_, Store>,
    id: String,
) -> Result<DownloadResult, String> {
    download::download_and_install(app, &store, &id).await
}

/// Запускає завантажений .exe через оболонку ОС (працює і для інсталяторів, що просять права адміністратора).
#[tauri::command]
pub fn run_exe(app: AppHandle, file_path: String) -> Result<bool, String> {
    app.opener()
        .open_path(file_path, None::<&str>)
        .map_err(|e| format!("Не вдалося запустити файл: {e}"))?;
    Ok(true)
}

#[tauri::command]
pub fn open_store_folder(app: AppHandle, store: State<'_, Store>) -> Result<(), String> {
    app.opener()
        .open_path(store.dir().to_string_lossy().to_string(), None::<&str>)
        .map_err(|e| e.to_string())
}

#[tauri::command]
pub fn open_path(app: AppHandle, target_path: String) -> Result<(), String> {
    if target_path.is_empty() {
        return Ok(());
    }
    app.opener()
        .open_path(target_path, None::<&str>)
        .map_err(|e| e.to_string())
}

#[tauri::command]
pub fn open_external(app: AppHandle, target_url: String) -> bool {
    if target_url.is_empty() {
        return false;
    }
    let Ok(parsed) = reqwest::Url::parse(&target_url) else {
        return false;
    };
    if !matches!(parsed.scheme(), "http" | "https") {
        return false;
    }
    app.opener().open_url(parsed.to_string(), None::<&str>).is_ok()
}
