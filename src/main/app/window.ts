import { app, BrowserWindow } from 'electron';
import path from 'path';

let mainWindow: BrowserWindow | null = null;

export const getMainWindow = () => mainWindow;

export const sendToRenderer = (channel: string, payload: unknown) => {
  mainWindow?.webContents.send(channel, payload);
};

export const createWindow = async () => {
  mainWindow = new BrowserWindow({
    width: 1100,
    height: 760,
    minWidth: 980,
    minHeight: 680,
    backgroundColor: '#0f121a',
    titleBarStyle: 'default',
    webPreferences: {
      preload: path.join(__dirname, '../preload/index.js'),
      contextIsolation: true,
      nodeIntegration: false
    }
  });

  mainWindow.setMenuBarVisibility(false);

  mainWindow.webContents.on('did-fail-load', (_event, code, desc, url) => {
    console.error(`[main] Failed to load ${url}: ${code} ${desc}`);
  });

  mainWindow.webContents.on('render-process-gone', (_event, details) => {
    console.error('[main] Renderer process gone:', details);
  });

  const devServerUrl = process.env.VITE_DEV_SERVER_URL || process.env.ELECTRON_RENDERER_URL;

  if (devServerUrl) {
    console.log(`[main] Loading renderer from ${devServerUrl}`);
    await mainWindow.loadURL(devServerUrl);
  } else if (!app.isPackaged) {
    const fallbackUrl = 'http://localhost:5173';
    console.warn(`[main] VITE_DEV_SERVER_URL missing, trying ${fallbackUrl}`);
    await mainWindow.loadURL(fallbackUrl);
  } else {
    await mainWindow.loadFile(path.join(__dirname, '../renderer/index.html'));
  }

  if (!app.isPackaged) {
    mainWindow.webContents.openDevTools({ mode: 'detach' });
  }

  return mainWindow;
};
