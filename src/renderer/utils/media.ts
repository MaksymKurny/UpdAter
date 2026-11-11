export const resolveImageUrl = (value?: string) => {
  if (!value) return "";
  if (
    value.startsWith("http://") ||
    value.startsWith("https://") ||
    value.startsWith("media://")
  ) {
    return value;
  }

  const toMediaUrl = (filePath: string) =>
    `media://local/${encodeURIComponent(filePath.replace(/\\/g, "/"))}`;

  if (value.startsWith("file://")) {
    const withoutScheme = value.slice("file://".length);
    const normalized = decodeURIComponent(withoutScheme).replace(/^\/+/, "");
    return toMediaUrl(normalized);
  }

  if (/^[a-zA-Z]:[\\/]/.test(value)) {
    return toMediaUrl(value);
  }

  return value;
};
