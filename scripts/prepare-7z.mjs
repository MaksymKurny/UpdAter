// Копіює 7z.exe + 7z.dll (повна збірка з підтримкою rar/iso/тощо) у src-tauri/resources/7z,
// звідки Tauri бере їх як ресурси застосунку.
import { cpSync, existsSync, mkdirSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const src = join(root, 'node_modules', '7zip-bin-full', 'win', 'x64');
const dest = join(root, 'src-tauri', 'resources', '7z');

if (!existsSync(src)) {
  console.error(`[prepare-7z] Не знайдено ${src}. Виконайте "pnpm install".`);
  process.exit(1);
}

mkdirSync(dest, { recursive: true });
for (const file of ['7z.exe', '7z.dll']) {
  cpSync(join(src, file), join(dest, file));
}
console.log('[prepare-7z] 7z скопійовано в', dest);
