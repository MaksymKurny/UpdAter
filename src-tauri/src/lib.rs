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

pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_dialog::init())
        .plugin(tauri_plugin_opener::init())
        .plugin(tauri_plugin_process::init())
        .setup(|app| {
            let data_dir = app.path().app_data_dir()?;
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
