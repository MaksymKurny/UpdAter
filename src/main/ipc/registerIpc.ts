import { dialog, ipcMain } from 'electron';
import type { TranslationEntry, TranslationInput } from '../../shared/types';
import { getMainWindow, sendToRenderer } from '../app/window';
import { openExternal, openPath, openStoreFolder, runExe } from '../app/openers';
import {
  addTranslation,
  getTranslations,
  removeTranslation,
  toggleFavorite,
  updateTranslation
} from '../store/settingsStore';
import { downloadAndInstall } from '../services/download';

export const registerIpcHandlers = () => {
  ipcMain.handle('translations:get', () => getTranslations());

  ipcMain.handle('translations:add', async (_event, input: TranslationInput) => addTranslation(input));

  ipcMain.handle('translations:update', (_event, payload: { id: string; patch: Partial<TranslationEntry> }) =>
    updateTranslation(payload.id, payload.patch)
  );

  ipcMain.handle('translations:remove', (_event, id: string) => removeTranslation(id));

  ipcMain.handle('translations:toggleFavorite', (_event, id: string) => toggleFavorite(id));

  ipcMain.handle('dialog:selectFolder', async () => {
    const mainWindow = getMainWindow();
    if (!mainWindow) return null;
    const result = await dialog.showOpenDialog(mainWindow, {
      properties: ['openDirectory', 'createDirectory']
    });
    if (result.canceled || result.filePaths.length === 0) return null;
    return result.filePaths[0];
  });

  ipcMain.handle('translations:downloadAndInstall', (_event, id: string) =>
    downloadAndInstall(id, {
      onProgress: (payload) => sendToRenderer('download-progress', payload),
      onComplete: (payload) => sendToRenderer('download-complete', payload)
    })
  );

  ipcMain.handle('app:runExe', async (_event, filePath: string) => {
    await runExe(filePath);
    return true;
  });

  ipcMain.handle('app:openStoreFolder', async () => {
    await openStoreFolder();
  });

  ipcMain.handle('app:openPath', async (_event, targetPath: string) => {
    await openPath(targetPath);
  });

  ipcMain.handle('app:openExternal', async (_event, targetUrl: string) => openExternal(targetUrl));
};
