import { convertFileSrc } from "@tauri-apps/api/core";

export const resolveImageUrl = (value?: string | null) => {
  if (!value) return "";
  if (
    value.startsWith("http://") ||
    value.startsWith("https://") ||
    value.startsWith("data:") ||
    value.startsWith("asset:") ||
    value.startsWith("http://asset.localhost")
  ) {
    return value;
  }

  // Локальний файл (наприклад, кеш Steam) - віддаємо через asset-протокол Tauri.
  if (value.startsWith("file://")) {
    const withoutScheme = value.slice("file://".length);
    return convertFileSrc(decodeURIComponent(withoutScheme).replace(/^\/+(?=[a-zA-Z]:)/, ""));
  }

  if (/^[a-zA-Z]:[\\/]/.test(value) || value.startsWith("/")) {
    return convertFileSrc(value);
  }

  return value;
};
