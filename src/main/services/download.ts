import { app } from 'electron';
import { createWriteStream, promises as fs } from 'fs';
import path from 'path';
import { pipeline } from 'stream/promises';
import { Readable, Transform } from 'stream';
import { extractFull } from 'node-7z';
import { path7z } from '7zip-bin-full';
import type { DownloadProgress, DownloadResult } from '../../shared/types';
import { getTranslations, updateTranslation } from '../store/settingsStore';
import { sanitizeFileName } from '../utils/path';

const archiveExtensions = [
  '.zip',
  '.7z',
  '.rar',
  '.tar',
  '.tar.gz',
  '.tgz',
  '.tar.bz2',
  '.tbz',
  '.tbz2',
  '.tar.xz',
  '.txz',
  '.gz',
  '.bz2',
  '.xz',
  '.iso'
];

const isArchive = (filePath: string) => {
  const lower = filePath.toLowerCase();
  return archiveExtensions.some((ext) => lower.endsWith(ext));
};

const isExe = (filePath: string) => filePath.toLowerCase().endsWith('.exe');

const extractDriveId = (url: string) => {
  const direct = url.match(/\/d\/([a-zA-Z0-9_-]+)/);
  if (direct?.[1]) return direct[1];
  try {
    const parsed = new URL(url);
    const id = parsed.searchParams.get('id');
    if (id) return id;
  } catch {
    return null;
  }
  return null;
};

const isGoogleDriveUrl = (url: string) => /drive\.google\.com|docs\.google\.com/.test(url);

const normalizeDriveHtml = (html: string) =>
  html.replace(/\\u003d/g, '=').replace(/\\u0026/g, '&').replace(/&amp;/g, '&');

const parseDriveConfirmToken = (html: string) => {
  const normalized = normalizeDriveHtml(html);
  return (
    normalized.match(/confirm=([0-9A-Za-z_-]+)/)?.[1] ||
    normalized.match(/name="confirm" value="([^"]+)"/)?.[1] ||
    normalized.match(/"confirm"\s*:\s*"([^"]+)"/)?.[1] ||
    null
  );
};

const extractDriveDownloadUrl = (html: string) => {
  const normalized = normalizeDriveHtml(html);
  const directUrl =
    normalized.match(/https?:\/\/drive\.usercontent\.google\.com\/download\?[^"'\\s>]+/)?.[0] ||
    normalized.match(/https?:\/\/drive\.google\.com\/uc\?export=download[^"'\\s>]+/)?.[0];
  if (directUrl) return directUrl;

  const formAction =
    normalized.match(/<form[^>]+id="download-form"[^>]+action="([^"]+)"/)?.[1] ||
    normalized.match(/<form[^>]+action="([^"]+drive\.usercontent\.google\.com\/download[^"]*)"/)?.[1];
  if (!formAction) return null;

  const inputs = Array.from(normalized.matchAll(/<input[^>]+name="([^"]+)"[^>]+value="([^"]*)"/g));
  const params = new URLSearchParams();
  for (const [, name, value] of inputs) {
    if (!name) continue;
    params.set(name, value ?? '');
  }
  const query = params.toString();
  return query ? `${formAction}?${query}` : formAction;
};

const isDriveAccessDeniedHtml = (html: string) => {
  const normalized = normalizeDriveHtml(html);
  return (
    /accounts\.google\.com|ServiceLogin/i.test(normalized) ||
    /request access|need access|access denied|permission/i.test(normalized) ||
    /потрібен доступ|запит доступу|немає доступу/i.test(normalized)
  );
};

const resolveGoogleDrive = async (url: string) => {
  const id = extractDriveId(url);
  if (!id) return { finalUrl: url };
  const base = `https://drive.google.com/uc?export=download&id=${id}`;
  const initial = await fetch(base, { redirect: 'manual' });

  const redirect = initial.headers.get('location');
  if (redirect) {
    return { finalUrl: redirect };
  }

  const cookie = initial.headers.get('set-cookie') || '';
  const cookieConfirm = cookie.match(/download_warning[^=]*=([^;]+)/)?.[1] || null;

  const contentType = initial.headers.get('content-type') || '';
  if (!contentType.includes('text/html')) {
    const headers = cookie ? { Cookie: cookie } : undefined;
    return { finalUrl: base, headers };
  }

  const html = await initial.text();
  const directUrl = extractDriveDownloadUrl(html);
  if (directUrl) {
    const headers = cookie ? { Cookie: cookie } : undefined;
    return { finalUrl: directUrl, headers };
  }

  const confirm = cookieConfirm || parseDriveConfirmToken(html);

  if (!confirm) {
    const headers = cookie ? { Cookie: cookie } : undefined;
    return { finalUrl: base, headers };
  }

  const headers = cookie ? { Cookie: cookie } : undefined;
  return { finalUrl: `${base}&confirm=${confirm}`, headers };
};

const resolveDownloadUrl = async (url: string) => {
  if (isGoogleDriveUrl(url)) {
    return resolveGoogleDrive(url);
  }
  return { finalUrl: url };
};

const getFileNameFromHeaders = (response: Response, url: string) => {
  const disposition = response.headers.get('content-disposition');
  if (disposition) {
    const match = disposition.match(/filename\*?=(?:UTF-8'')?["']?([^"';]+)["']?/i);
    if (match?.[1]) return decodeURIComponent(match[1]);
  }
  try {
    const parsed = new URL(url);
    const base = path.basename(parsed.pathname);
    if (base && base !== '/') return base;
  } catch {
    return `download-${Date.now()}`;
  }
  return `download-${Date.now()}`;
};

const downloadFile = async (
  url: string,
  destDir: string,
  onProgress?: (progress: Omit<DownloadProgress, 'id'>) => void
) => {
  const { finalUrl, headers } = await resolveDownloadUrl(url);
  let response = await fetch(finalUrl, { headers, redirect: 'follow' });

  if (!response.ok) {
    throw new Error(`Download failed: ${response.status} ${response.statusText}`);
  }

  const initialContentType = response.headers.get('content-type') || '';
  if (initialContentType.includes('text/html') && isGoogleDriveUrl(url)) {
    const html = await response.text();
    if (isDriveAccessDeniedHtml(html)) {
      throw new Error('Google Drive: файл недоступний. Перевірте, чи відкрито доступ для всіх.');
    }
    const retryUrl = extractDriveDownloadUrl(html);
    if (retryUrl && retryUrl !== finalUrl) {
      response = await fetch(retryUrl, { headers, redirect: 'follow' });
      if (!response.ok) {
        throw new Error(`Download failed: ${response.status} ${response.statusText}`);
      }
    } else {
      const confirm = parseDriveConfirmToken(html);
      if (confirm) {
        const id = extractDriveId(url);
        if (id) {
          const fallbackUrl = `https://drive.google.com/uc?export=download&id=${id}&confirm=${confirm}`;
          response = await fetch(fallbackUrl, { headers, redirect: 'follow' });
          if (!response.ok) {
            throw new Error(`Download failed: ${response.status} ${response.statusText}`);
          }
        }
      }
    }
  }

  const fileName = sanitizeFileName(getFileNameFromHeaders(response, finalUrl));
  const filePath = path.join(destDir, `${Date.now()}-${fileName}`);

  if (!response.body) {
    throw new Error('Empty response body.');
  }

  const contentType = response.headers.get('content-type') || '';
  if (contentType.includes('text/html') && isGoogleDriveUrl(url)) {
    throw new Error('Google Drive повернув HTML підтвердження. Перевірте доступ або посилання.');
  }

  const total = Number(response.headers.get('content-length') || 0);
  let received = 0;

  const progressStream = new Transform({
    transform(chunk, _enc, callback) {
      received += chunk.length;
      onProgress?.({
        received,
        total,
        percent: total ? Math.round((received / total) * 100) : undefined
      });
      callback(null, chunk);
    }
  });

  const readable = Readable.fromWeb(response.body as any);
  await pipeline(readable, progressStream, createWriteStream(filePath));

  return { filePath, fileName };
};

const extractArchive = async (archivePath: string, dest: string) => {
  try {
    await fs.access(path7z);
  } catch {
    throw new Error(`Не знайдено 7z: ${path7z}`);
  }
  await fs.mkdir(dest, { recursive: true });
  await new Promise<void>((resolve, reject) => {
    const stream = extractFull(archivePath, dest, { $bin: path7z, overwrite: 'a' });
    stream.on('end', () => resolve());
    stream.on('error', (error) => reject(error));
  });
};

export type DownloadCallbacks = {
  onProgress?: (payload: DownloadProgress) => void;
  onComplete?: (payload: DownloadResult) => void;
};

export const downloadAndInstall = async (id: string, callbacks: DownloadCallbacks = {}) => {
  const item = getTranslations().find((entry) => entry.Id === id);
  if (!item) throw new Error('Переклад не знайдено.');
  if (!item.Path) throw new Error('Не вказано шлях встановлення.');

  const downloadDir = path.join(app.getPath('userData'), 'downloads', id);
  await fs.mkdir(downloadDir, { recursive: true });

  const result = await downloadFile(item.Url, downloadDir, (progress) => {
    callbacks.onProgress?.({
      id,
      received: progress.received,
      total: progress.total,
      percent: progress.percent
    });
  });

  let extracted = false;
  let copied = false;
  const exe = isExe(result.filePath);

  if (isArchive(result.filePath)) {
    await extractArchive(result.filePath, item.Path);
    extracted = true;
  } else if (!exe) {
    await fs.mkdir(item.Path, { recursive: true });
    await fs.copyFile(result.filePath, path.join(item.Path, result.fileName));
    copied = true;
  }

  updateTranslation(id, { LastUpdate: new Date().toISOString() });

  const payload: DownloadResult = {
    id,
    filePath: result.filePath,
    fileName: result.fileName,
    isExe: exe,
    extracted,
    copied
  };

  callbacks.onComplete?.(payload);
  return payload;
};
