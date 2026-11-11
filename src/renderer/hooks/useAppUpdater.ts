import { useCallback, useEffect, useRef, useState } from "react";
import { check, type Update } from "@tauri-apps/plugin-updater";
import { relaunch } from "@tauri-apps/plugin-process";

export type UpdaterStatus =
  | "idle"
  | "available"
  | "downloading"
  | "ready"
  | "error";

export type AppUpdaterState = {
  status: UpdaterStatus;
  version?: string;
  percent?: number;
  error?: string;
};

/**
 * Перевіряє наявність нової версії програми при запуску (GitHub Releases)
 * і дозволяє встановити її одним кліком.
 */
export function useAppUpdater() {
  const [state, setState] = useState<AppUpdaterState>({ status: "idle" });
  const updateRef = useRef<Update | null>(null);

  useEffect(() => {
    let cancelled = false;
    check()
      .then((update) => {
        if (cancelled || !update) return;
        updateRef.current = update;
        setState({ status: "available", version: update.version });
      })
      .catch((error) => {
        // Немає мережі / ключ не налаштовано / плагін вимкнено - не турбуємо користувача.
        console.warn("[updater] Перевірку оновлень пропущено:", error);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  const install = useCallback(async () => {
    const update = updateRef.current;
    if (!update) return;
    let total = 0;
    let received = 0;
    setState({ status: "downloading", version: update.version, percent: 0 });
    try {
      await update.downloadAndInstall((event) => {
        if (event.event === "Started") {
          total = event.data.contentLength ?? 0;
        } else if (event.event === "Progress") {
          received += event.data.chunkLength;
          const percent = total ? Math.min(100, Math.round((received / total) * 100)) : undefined;
          setState((prev) => ({ ...prev, percent }));
        } else if (event.event === "Finished") {
          setState((prev) => ({ ...prev, status: "ready", percent: 100 }));
        }
      });
      await relaunch();
    } catch (error) {
      setState({
        status: "error",
        version: update.version,
        error: error instanceof Error ? error.message : String(error),
      });
    }
  }, []);

  const dismiss = useCallback(() => setState({ status: "idle" }), []);

  return { state, install, dismiss };
}
