import { execFile } from 'child_process';

export const execFileAsync = (file: string, args: string[]) =>
  new Promise<{ stdout: string; stderr: string }>((resolve, reject) => {
    execFile(file, args, { windowsHide: true }, (error, stdout, stderr) => {
      if (error) {
        reject(error);
        return;
      }
      resolve({ stdout: String(stdout), stderr: String(stderr) });
    });
  });

export const queryRegistryValue = async (key: string, valueName: string) => {
  if (process.platform !== 'win32') return null;
  try {
    const { stdout } = await execFileAsync('reg', ['query', key, '/v', valueName]);
    const lines = stdout.split(/\r?\n/).map((line) => line.trim()).filter(Boolean);
    for (const line of lines) {
      if (line.toLowerCase().startsWith(valueName.toLowerCase())) {
        const parts = line.split(/\s{2,}/);
        return parts[2]?.trim() || null;
      }
    }
  } catch {
    return null;
  }
  return null;
};
