# UpdAter (Tauri)

UpdAter - програма для швидкого й автоматичного оновлення українізаторів до ігор.
Ця гілка - версія на **Tauri v2** (Rust + React/Vite) замість Electron.

## Структура

- `src/renderer` - React-інтерфейс (Vite). Звертається до бекенда через `src/renderer/api.ts`
  (той самий `window.updater`, що був у Electron preload).
- `src-tauri/src` - Rust-бекенд:
  - `store.rs` - список перекладів у `settings.json` (формат `{ "List": [...] }`);
  - `download.rs` - завантаження (зокрема Google Drive), прогрес, розпаковка архівів через 7-Zip;
  - `steam.rs` - пошук іконки й банера гри в кеші Steam (реєстр Windows + `appmanifest`);
  - `commands.rs` - команди, які викликає фронтенд.
- `scripts/prepare-7z.mjs` - кладе `7z.exe`/`7z.dll` у ресурси застосунку (для rar, iso, 7z тощо).

## Розробка

Потрібні Node 20+, pnpm, Rust (stable) і [залежності Tauri](https://tauri.app/start/prerequisites/).

```bash
pnpm install
pnpm dev          # tauri dev
pnpm typecheck    # перевірка TypeScript
cargo test --manifest-path src-tauri/Cargo.toml
```

## Збірка

```bash
pnpm build        # NSIS-інсталятор у src-tauri/target/release/bundle/nsis
```

Локально, без ключа підпису оновлень, це працює як є. Підписані артефакти оновлення
створює лише релізний конфіг (`src-tauri/tauri.release.conf.json`).

## Автооновлення

Програма при запуску перевіряє `latest.json` в останньому GitHub Release і пропонує оновитись
(`tauri-plugin-updater`). Налаштування один раз:

1. Згенеруйте ключі: `pnpm tauri signer generate -w ~/.tauri/updater.key`.
2. Вставте **публічний** ключ у `src-tauri/tauri.conf.json` -> `plugins.updater.pubkey`.
   Поки там заглушка, автооновлення вимкнене (програма працює, лише не перевіряє оновлення).
3. У репозиторії GitHub додайте секрети `TAURI_SIGNING_PRIVATE_KEY` (вміст приватного ключа)
   і, якщо задавали пароль, `TAURI_SIGNING_PRIVATE_KEY_PASSWORD`. **Приватний ключ нікуди не комітьте.**

## Випуск нової версії

1. Підніміть версію в `package.json`, `src-tauri/Cargo.toml` і `src-tauri/tauri.conf.json`.
2. Створіть і запушіть тег: `git tag v0.1.1 && git push origin v0.1.1`.
3. Workflow `Release` збере інсталятор, підпише його й опублікує реліз разом із `latest.json`.

## Дані користувача

Застосунок портативний: список перекладів зберігається в `settings.json` **поруч з exe**.
Якщо в теку exe писати не можна (наприклад, `Program Files`), використовується
`%APPDATA%\com.updater.app`. Теку можна задати змінною середовища `UPDATER_DATA_DIR`.

Тестові дані: `pnpm dev:sample` запускає застосунок із прикладами різних випадків
(`test-data/settings.sample.json`) у робочій копії `test-data/run`, яка щоразу створюється
наново. Завантаження йде в тимчасову теку `.updater-tmp-*`
всередині вказаної папки встановлення; після розпаковки вміст переноситься з заміною
наявних файлів, а тимчасова тека видаляється.
