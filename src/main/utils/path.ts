import path from 'path';

export const sanitizeFileName = (name: string) => name.replace(/[<>:"/\\|?*]+/g, '_');

export const normalizeFsPath = (value: string) =>
  path.normalize(value).replace(/[\\/]+$/g, '').toLowerCase();
