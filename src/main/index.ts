import { app, BrowserWindow } from 'electron';
import { registerMediaProtocol, registerMediaScheme } from './app/mediaProtocol';
import { createWindow } from './app/window';
import { registerIpcHandlers } from './ipc/registerIpc';

registerMediaScheme();

app.whenReady().then(async () => {
  app.setAppUserModelId('com.updater.app');

  registerMediaProtocol();
  await createWindow();
  registerIpcHandlers();

  app.on('activate', async () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      await createWindow();
    }
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});
