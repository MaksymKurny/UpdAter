// Запускає `tauri dev` з тестовим набором записів з test-data/settings.sample.json.
// Щоразу створює свіжу робочу копію в test-data/run (її можна безболісно змінювати),
// підставляє в шляхи {{DATA}} -> абсолютний шлях до test-data/run і вказує застосунку
// брати settings.json звідти через UPDATER_DATA_DIR. Ваш справжній settings.json не чіпається.
import { spawn } from 'node:child_process';
import { mkdirSync, readFileSync, rmSync, writeFileSync } from 'node:fs';
import { dirname, join, sep } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const runDir = join(root, 'test-data', 'run');
const sample = JSON.parse(readFileSync(join(root, 'test-data', 'settings.sample.json'), 'utf8'));

const replaceData = (value) => {
  if (typeof value === 'string') {
    if (!value.includes('{{DATA}}')) return value;
    // Шлях у зразку пишемо через "/", тут приводимо до рідних роздільників ОС.
    return value.replaceAll('{{DATA}}', runDir).replaceAll('/', sep);
  }
  if (Array.isArray(value)) return value.map(replaceData);
  if (value && typeof value === 'object') {
    return Object.fromEntries(Object.entries(value).map(([k, v]) => [k, replaceData(v)]));
  }
  return value;
};

rmSync(runDir, { recursive: true, force: true });
mkdirSync(runDir, { recursive: true });
writeFileSync(join(runDir, 'settings.json'), JSON.stringify(replaceData(sample), null, 2));
console.log('[dev-sample] Робоча копія:', join(runDir, 'settings.json'));

const extraArgs = process.argv.slice(2);
if (extraArgs.includes('--prepare-only')) process.exit(0);

const child = spawn('pnpm', ['tauri', 'dev', ...extraArgs], {
  cwd: root,
  stdio: 'inherit',
  shell: true,
  env: { ...process.env, UPDATER_DATA_DIR: runDir },
});
child.on('exit', (code) => process.exit(code ?? 0));
