import type { DownloadProgress, DownloadResult, TranslationEntry, TranslationInput } from '../shared/types';

declare global {
  interface Window {
    updater: {
      getTranslations: () => Promise<TranslationEntry[]>;
      addTranslation: (input: TranslationInput) => Promise<TranslationEntry>;
      updateTranslation: (id: string, patch: Partial<TranslationInput>) => Promise<TranslationEntry | null>;
      removeTranslation: (id: string) => Promise<boolean>;
      toggleFavorite: (id: string) => Promise<TranslationEntry | null>;
      selectFolder: () => Promise<string | null>;
      downloadAndInstall: (id: string) => Promise<DownloadResult>;
      runExe: (filePath: string) => Promise<boolean>;
      openStoreFolder: () => Promise<void>;
      openPath: (targetPath: string) => Promise<void>;
      openExternal: (targetUrl: string) => Promise<boolean>;
      onDownloadProgress: (handler: (payload: DownloadProgress) => void) => () => void;
      onDownloadComplete: (handler: (payload: DownloadResult) => void) => () => void;
    };
  }
}

export {};
