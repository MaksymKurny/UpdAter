import { invoke } from "@tauri-apps/api/core";
import { listen } from "@tauri-apps/api/event";
import { open } from "@tauri-apps/plugin-dialog";
import type {
  DownloadProgress,
  DownloadResult,
  TranslationEntry,
  TranslationInput,
} from "../shared/types";

// Tauri повертає помилки командами як рядки - перетворюємо на Error,
// щоб решта UI (`err instanceof Error`) працювала як раніше.
const call = async <T>(command: string, args?: Record<string, unknown>): Promise<T> => {
  try {
    return await invoke<T>(command, args);
  } catch (error) {
    throw error instanceof Error ? error : new Error(String(error));
  }
};

const subscribe = <T>(event: string, handler: (payload: T) => void) => {
  const pending = listen<T>(event, (e) => handler(e.payload));
  return () => {
    pending.then((unlisten) => unlisten());
  };
};

/** Той самий інтерфейс, що раніше надавав Electron preload (`window.updater`). */
export const updater: Window["updater"] = {
  getTranslations: () => call<TranslationEntry[]>("get_translations"),
  addTranslation: (input: TranslationInput) => call<TranslationEntry>("add_translation", { input }),
  updateTranslation: (id, patch) =>
    call<TranslationEntry | null>("update_translation", { id, patch }),
  removeTranslation: (id) => call<boolean>("remove_translation", { id }),
  toggleFavorite: (id) => call<TranslationEntry | null>("toggle_favorite", { id }),
  selectFolder: async () => {
    const selected = await open({ directory: true, multiple: false });
    return typeof selected === "string" ? selected : null;
  },
  selectImage: async () => {
    const selected = await open({
      multiple: false,
      filters: [{ name: "Зображення", extensions: ["png", "jpg", "jpeg", "webp", "gif", "bmp", "ico"] }],
    });
    return typeof selected === "string" ? selected : null;
  },
  downloadAndInstall: (id) => call<DownloadResult>("download_and_install", { id }),
  runExe: (filePath) => call<boolean>("run_exe", { filePath }),
  openStoreFolder: () => call<void>("open_store_folder"),
  openPath: (targetPath) => call<void>("open_path", { targetPath }),
  openExternal: (targetUrl) => call<boolean>("open_external", { targetUrl }),
  onDownloadProgress: (handler) => subscribe<DownloadProgress>("download-progress", handler),
  onDownloadComplete: (handler) => subscribe<DownloadResult>("download-complete", handler),
};
