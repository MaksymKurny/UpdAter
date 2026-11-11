import { app } from 'electron';
import { promises as fs } from 'fs';
import path from 'path';
import { fileExists } from '../utils/fs';
import { normalizeFsPath } from '../utils/path';
import { queryRegistryValue } from '../utils/registry';

const logSteam = (...args: unknown[]) => {
  if (!app.isPackaged) {
    console.log('[steam]', ...args);
  }
};

let cachedSteamPath: string | null = null;
let cachedLibraryPaths: string[] | null = null;

const getSteamInstallPath = async () => {
  if (cachedSteamPath) return cachedSteamPath;
  if (process.platform !== 'win32') return null;
  const candidates = [
    { key: 'HKCU\\Software\\Valve\\Steam', value: 'SteamPath' },
    { key: 'HKCU\\Software\\Valve\\Steam', value: 'SteamExe' },
    { key: 'HKLM\\SOFTWARE\\WOW6432Node\\Valve\\Steam', value: 'InstallPath' },
    { key: 'HKLM\\SOFTWARE\\Valve\\Steam', value: 'InstallPath' }
  ];
  for (const candidate of candidates) {
    const raw = await queryRegistryValue(candidate.key, candidate.value);
    if (!raw) continue;
    const cleaned = raw.replace(/"/g, '').trim();
    const resolved = candidate.value === 'SteamExe' ? path.dirname(cleaned) : cleaned;
    if (resolved) {
      cachedSteamPath = resolved;
      return resolved;
    }
  }
  return null;
};

const getSteamLibraryPaths = async (steamPath: string) => {
  if (cachedLibraryPaths) return cachedLibraryPaths;
  const libraries = new Set<string>();
  libraries.add(steamPath);

  const libraryFile = path.join(steamPath, 'steamapps', 'libraryfolders.vdf');
  if (await fileExists(libraryFile)) {
    const raw = await fs.readFile(libraryFile, 'utf-8');
    const matches = raw.matchAll(/"path"\s+"([^"]+)"/g);
    for (const match of matches) {
      const rawPath = match[1];
      if (!rawPath) continue;
      const unescaped = rawPath.replace(/\\\\/g, '\\');
      libraries.add(unescaped);
    }
  }

  cachedLibraryPaths = Array.from(libraries);
  return cachedLibraryPaths;
};

const parseAppManifest = (content: string) => {
  const appid = content.match(/"appid"\s+"(\d+)"/i)?.[1] || null;
  const installdir = content.match(/"installdir"\s+"([^"]+)"/i)?.[1] || null;
  return { appid, installdir };
};

const findSteamAppIdByPath = async (gamePath: string, libraryPaths: string[]) => {
  const normalizedGamePath = normalizeFsPath(gamePath);
  for (const libraryPath of libraryPaths) {
    const steamappsPath = path.join(libraryPath, 'steamapps');
    let entries: string[];
    try {
      entries = await fs.readdir(steamappsPath);
    } catch {
      continue;
    }

    const manifests = entries.filter((name) => /^appmanifest_\d+\.acf$/i.test(name));
    for (const manifest of manifests) {
      const manifestPath = path.join(steamappsPath, manifest);
      let content = '';
      try {
        content = await fs.readFile(manifestPath, 'utf-8');
      } catch {
        continue;
      }

      const { appid, installdir } = parseAppManifest(content);
      if (!appid || !installdir) continue;

      const installPath = normalizeFsPath(path.join(libraryPath, 'steamapps', 'common', installdir));
      if (normalizedGamePath.startsWith(installPath)) {
        return appid;
      }
    }
  }
  return null;
};

const pickFirstExisting = async (basePath: string, candidates: string[]) => {
  for (const candidate of candidates) {
    const fullPath = path.join(basePath, candidate);
    if (await fileExists(fullPath)) {
      return fullPath;
    }
  }
  return null;
};

const pickFirstByExtensions = async (basePath: string, extensions: string[]) => {
  try {
    const entries = await fs.readdir(basePath);
    for (const entry of entries) {
      const ext = path.extname(entry).toLowerCase();
      if (extensions.includes(ext)) {
        return path.join(basePath, entry);
      }
    }
  } catch {
    return null;
  }
  return null;
};

export const resolveSteamMediaForPath = async (gamePath: string) => {
  if (!/steamapps|steamlibrary/i.test(gamePath)) return null;
  logSteam('Checking path:', gamePath);
  const steamPath = await getSteamInstallPath();
  if (!steamPath) {
    logSteam('Steam path not found in registry.');
    return null;
  }
  logSteam('Steam path:', steamPath);

  const libraryPaths = await getSteamLibraryPaths(steamPath);
  logSteam('Library paths:', libraryPaths);
  const appId = await findSteamAppIdByPath(gamePath, libraryPaths);
  if (!appId) {
    logSteam('AppId not found for path.');
    return null;
  }
  logSteam('Resolved appId:', appId);

  const cacheDir = path.join(steamPath, 'appcache', 'librarycache', appId);
  if (!(await fileExists(cacheDir))) {
    logSteam('Cache directory missing:', cacheDir);
    return null;
  }
  logSteam('Cache directory:', cacheDir);

  const bannerCandidates = ['library_hero.jpg', 'library_hero_blur.jpg', 'header.jpg', 'library_hero.png'];
  const iconCandidates = [
    'library_600x900.jpg',
    'library_600x900_2x.jpg',
    'capsule_231x87.jpg',
    'capsule_231x87.png',
    'logo.png'
  ];

  const banner = await pickFirstExisting(cacheDir, bannerCandidates);
  const icon = await pickFirstExisting(cacheDir, iconCandidates);

  const fallback = await pickFirstByExtensions(cacheDir, ['.jpg', '.png']);
  logSteam('Resolved media:', { banner, icon, fallback });
  return {
    banner: banner ?? fallback ?? undefined,
    icon: icon ?? fallback ?? undefined
  };
};
