mod commands;
mod download;
mod models;
mod steam;
mod store;

use tauri::Manager;

/// Публічні ключі, створені `tauri signer generate`, починаються з цього префікса
/// (base64 від "untrusted comment"). Поки у конфігу заглушка - автооновлення вимкнене.
const PUBKEY_PREFIX: &str = "dW50cnVzdGVk";

fn updater_pubkey_configured(app: &tauri::App) -> bool {
    app.config()
        .plugins
        .0
        .get("updater")
        .and_then(|cfg| cfg.get("pubkey"))
        .and_then(|key| key.as_str())
        .map(|key| key.trim().starts_with(PUBKEY_PREFIX))
        .unwrap_or(false)
}

/// Тека для `settings.json`. Застосунок портативний, тому дані лежать поруч з exe.
/// Порядок: змінна `UPDATER_DATA_DIR` (для тестів) -> тека exe -> `AppData`, якщо в теку exe
/// писати не можна (наприклад, `Program Files`).
fn resolve_data_dir(app: &tauri::App) -> Result<std::path::PathBuf, Box<dyn std::error::Error>> {
    if let Some(custom) = std::env::var_os("UPDATER_DATA_DIR").filter(|v| !v.is_empty()) {
        return Ok(custom.into());
    }
    if let Some(exe_dir) = std::env::current_exe().ok().and_then(|exe| exe.parent().map(|p| p.to_path_buf())) {
        let probe = exe_dir.join(".updater-write-test");
        if std::fs::write(&probe, b"").is_ok() {
            let _ = std::fs::remove_file(&probe);
            return Ok(exe_dir);
        }
    }
    Ok(app.path().app_data_dir()?)
}

pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_dialog::init())
        .plugin(tauri_plugin_opener::init())
        .plugin(tauri_plugin_process::init())
        .setup(|app| {
            let data_dir = resolve_data_dir(app)?;
            app.manage(store::Store::load(&data_dir));

            if updater_pubkey_configured(app) {
                app.handle().plugin(tauri_plugin_updater::Builder::new().build())?;
            } else {
                eprintln!("[updater] Публічний ключ не налаштований у tauri.conf.json - автооновлення вимкнено.");
            }
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![
            commands::get_translations,
            commands::add_translation,
            commands::update_translation,
            commands::remove_translation,
            commands::toggle_favorite,
            commands::download_and_install,
            commands::run_exe,
            commands::open_store_folder,
            commands::open_path,
            commands::open_external,
        ])
        .run(tauri::generate_context!())
        .expect("помилка під час запуску UpdAter");
}
