import { shell } from 'electron';
import { spawn } from 'child_process';
import path from 'path';
import { getStorePath } from '../store/settingsStore';

export const runExe = async (filePath: string) => {
  await new Promise<void>((resolve, reject) => {
    const child = spawn(filePath, [], { detached: true, stdio: 'ignore', shell: true });
    child.on('error', reject);
    child.unref();
    resolve();
  });
};

export const openStoreFolder = async () => {
  const storePath = getStorePath();
  await shell.openPath(path.dirname(storePath));
};

export const openPath = async (targetPath: string) => {
  if (!targetPath) return;
  await shell.openPath(targetPath);
};

export const openExternal = async (targetUrl: string) => {
  if (!targetUrl) return false;
  try {
    const parsed = new URL(targetUrl);
    if (!['http:', 'https:'].includes(parsed.protocol)) {
      return false;
    }
    await shell.openExternal(parsed.toString());
    return true;
  } catch {
    return false;
  }
};
