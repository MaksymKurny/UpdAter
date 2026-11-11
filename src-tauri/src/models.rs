use serde::{Deserialize, Serialize};

/// Запис про переклад. Формат збігається з settings.json попередніх версій (PascalCase).
#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "PascalCase")]
pub struct TranslationEntry {
    pub id: String,
    pub title: String,
    pub url: String,
    #[serde(default, skip_serializing_if = "Option::is_none")]
    pub guide_url: Option<String>,
    pub path: String,
    #[serde(default, skip_serializing_if = "Option::is_none")]
    pub icon: Option<String>,
    #[serde(default, skip_serializing_if = "Option::is_none")]
    pub banner: Option<String>,
    #[serde(default, skip_serializing_if = "Option::is_none")]
    pub last_update: Option<String>,
    #[serde(default)]
    pub add_to_list: bool,
    #[serde(default)]
    pub pinned_state: bool,
    #[serde(default)]
    pub meta_info: Option<String>,
}

#[derive(Deserialize, Default, Debug)]
#[serde(rename_all = "PascalCase", default)]
pub struct TranslationInput {
    pub title: String,
    pub url: String,
    pub guide_url: Option<String>,
    pub path: String,
    pub icon: Option<String>,
    pub banner: Option<String>,
    pub meta_info: Option<String>,
    pub add_to_list: bool,
    pub pinned_state: bool,
}

#[derive(Serialize, Deserialize, Default)]
#[serde(rename_all = "PascalCase", default)]
pub struct SettingsFile {
    pub list: Vec<TranslationEntry>,
}

#[derive(Serialize, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct DownloadProgress {
    pub id: String,
    pub received: u64,
    pub total: Option<u64>,
    pub percent: Option<u8>,
}

#[derive(Serialize, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct DownloadResult {
    pub id: String,
    pub file_path: String,
    pub file_name: String,
    pub is_exe: bool,
    pub extracted: bool,
    pub copied: bool,
}
