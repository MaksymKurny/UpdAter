use std::fs;
use std::path::{Path, PathBuf};
use std::sync::{Mutex, MutexGuard};

use serde_json::{Map, Value};

use crate::models::{SettingsFile, TranslationEntry};

/// Список перекладів у `settings.json` (теж формат `{ "List": [...] }`).
pub struct Store {
    path: PathBuf,
    list: Mutex<Vec<TranslationEntry>>,
}

impl Store {
    pub fn load(dir: &Path) -> Self {
        let path = dir.join("settings.json");
        let list = fs::read_to_string(&path)
            .ok()
            .and_then(|raw| serde_json::from_str::<SettingsFile>(raw.trim_start_matches('\u{feff}')).ok())
            .map(|file| file.list)
            .unwrap_or_default();
        Self {
            path,
            list: Mutex::new(list),
        }
    }

    pub fn dir(&self) -> &Path {
        self.path.parent().unwrap_or_else(|| Path::new("."))
    }

    fn lock(&self) -> MutexGuard<'_, Vec<TranslationEntry>> {
        self.list.lock().unwrap_or_else(|e| e.into_inner())
    }

    fn persist(&self, list: &[TranslationEntry]) -> Result<(), String> {
        if let Some(parent) = self.path.parent() {
            fs::create_dir_all(parent).map_err(|e| format!("Не вдалося створити теку налаштувань: {e}"))?;
        }
        let body = serde_json::to_string_pretty(&SettingsFile { list: list.to_vec() })
            .map_err(|e| e.to_string())?;
        // Спочатку тимчасовий файл, щоб не втратити список при збої запису.
        let tmp = self.path.with_extension("json.tmp");
        fs::write(&tmp, body).map_err(|e| format!("Не вдалося зберегти налаштування: {e}"))?;
        fs::rename(&tmp, &self.path).map_err(|e| format!("Не вдалося зберегти налаштування: {e}"))?;
        Ok(())
    }

    pub fn get_all(&self) -> Vec<TranslationEntry> {
        self.lock().clone()
    }

    pub fn find(&self, id: &str) -> Option<TranslationEntry> {
        self.lock().iter().find(|e| e.id == id).cloned()
    }

    pub fn add(&self, entry: TranslationEntry) -> Result<TranslationEntry, String> {
        let mut list = self.lock();
        list.push(entry.clone());
        self.persist(&list)?;
        Ok(entry)
    }

    /// Застосовує часткове оновлення (як `{ ...old, ...patch }` в Electron-версії).
    pub fn update(&self, id: &str, patch: Map<String, Value>) -> Result<Option<TranslationEntry>, String> {
        let mut list = self.lock();
        let Some(index) = list.iter().position(|e| e.id == id) else {
            return Ok(None);
        };
        let mut value = serde_json::to_value(&list[index]).map_err(|e| e.to_string())?;
        if let Some(object) = value.as_object_mut() {
            for (key, new_value) in patch {
                // Id змінювати не можна.
                if key != "Id" {
                    object.insert(key, new_value);
                }
            }
        }
        let updated: TranslationEntry = serde_json::from_value(value).map_err(|e| e.to_string())?;
        list[index] = updated.clone();
        self.persist(&list)?;
        Ok(Some(updated))
    }

    pub fn remove(&self, id: &str) -> Result<bool, String> {
        let mut list = self.lock();
        let before = list.len();
        list.retain(|e| e.id != id);
        let removed = list.len() != before;
        self.persist(&list)?;
        Ok(removed)
    }

    pub fn toggle_favorite(&self, id: &str) -> Result<Option<TranslationEntry>, String> {
        let mut list = self.lock();
        let Some(entry) = list.iter_mut().find(|e| e.id == id) else {
            return Ok(None);
        };
        entry.pinned_state = !entry.pinned_state;
        let updated = entry.clone();
        self.persist(&list)?;
        Ok(Some(updated))
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn entry(id: &str) -> TranslationEntry {
        TranslationEntry {
            id: id.into(),
            title: "Game".into(),
            url: "https://example.com/a.zip".into(),
            guide_url: None,
            path: "C:/Games/Game".into(),
            icon: None,
            banner: None,
            last_update: None,
            add_to_list: false,
            pinned_state: false,
            meta_info: None,
        }
    }

    #[test]
    fn persists_updates_and_reloads() {
        let dir = std::env::temp_dir().join(format!("updater-store-{}", uuid::Uuid::new_v4()));
        let store = Store::load(&dir);
        store.add(entry("1")).unwrap();
        store.add(entry("2")).unwrap();

        let mut patch = Map::new();
        patch.insert("Title".into(), Value::String("Нова назва".into()));
        patch.insert("Id".into(), Value::String("hack".into()));
        let updated = store.update("1", patch).unwrap().unwrap();
        assert_eq!(updated.title, "Нова назва");
        assert_eq!(updated.id, "1");

        assert!(store.toggle_favorite("2").unwrap().unwrap().pinned_state);
        assert!(store.remove("1").unwrap());
        assert!(!store.remove("missing").unwrap());

        let raw = std::fs::read_to_string(dir.join("settings.json")).unwrap();
        assert!(raw.contains("\"List\"") && raw.contains("\"PinnedState\": true"));

        let reloaded = Store::load(&dir);
        assert_eq!(reloaded.get_all().len(), 1);
        assert_eq!(reloaded.get_all()[0].id, "2");
        let _ = std::fs::remove_dir_all(&dir);
    }
}
