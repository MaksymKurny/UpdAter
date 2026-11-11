import { contextBridge, ipcRenderer } from 'electron';
import type { DownloadProgress, DownloadResult, TranslationEntry, TranslationInput } from '../shared/types';

contextBridge.exposeInMainWorld('updater', {
  getTranslations: (): Promise<TranslationEntry[]> => ipcRenderer.invoke('translations:get'),
  addTranslation: (input: TranslationInput): Promise<TranslationEntry> =>
    ipcRenderer.invoke('translations:add', input),
  updateTranslation: (id: string, patch: Partial<TranslationInput>) =>
    ipcRenderer.invoke('translations:update', { id, patch }),
  removeTranslation: (id: string): Promise<boolean> => ipcRenderer.invoke('translations:remove', id),
  toggleFavorite: (id: string) => ipcRenderer.invoke('translations:toggleFavorite', id),
  selectFolder: (): Promise<string | null> => ipcRenderer.invoke('dialog:selectFolder'),
  downloadAndInstall: (id: string): Promise<DownloadResult> =>
    ipcRenderer.invoke('translations:downloadAndInstall', id),
  runExe: (filePath: string): Promise<boolean> => ipcRenderer.invoke('app:runExe', filePath),
  openStoreFolder: (): Promise<void> => ipcRenderer.invoke('app:openStoreFolder'),
  openPath: (targetPath: string): Promise<void> => ipcRenderer.invoke('app:openPath', targetPath),
  openExternal: (targetUrl: string): Promise<boolean> =>
    ipcRenderer.invoke('app:openExternal', targetUrl),
  onDownloadProgress: (handler: (payload: DownloadProgress) => void) => {
    const listener = (_event: Electron.IpcRendererEvent, payload: DownloadProgress) => handler(payload);
    ipcRenderer.on('download-progress', listener);
    return () => ipcRenderer.removeListener('download-progress', listener);
  },
  onDownloadComplete: (handler: (payload: DownloadResult) => void) => {
    const listener = (_event: Electron.IpcRendererEvent, payload: DownloadResult) => handler(payload);
    ipcRenderer.on('download-complete', listener);
    return () => ipcRenderer.removeListener('download-complete', listener);
  }
});

