import { useEffect, useMemo, useState } from "react";
import AddTranslationModal from "./components/AddTranslationModal";
import ErrorBanner from "./components/feedback/ErrorBanner";
import UpdateBanner from "./components/feedback/UpdateBanner";
import { useAppUpdater } from "./hooks/useAppUpdater";
import ConfirmModal from "./components/ConfirmModal";
import SearchRow from "./components/layout/SearchRow";
import TopBar from "./components/layout/TopBar";
import TranslationList from "./components/list/TranslationList";
import type {
  DownloadProgress,
  DownloadResult,
  TranslationEntry,
  TranslationInput,
} from "../shared/types";
import { toInput } from "./utils/translation";

export default function App() {
  const [translations, setTranslations] = useState<TranslationEntry[]>([]);
  const [query, setQuery] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<TranslationEntry | null>(null);
  const [removing, setRemoving] = useState<TranslationEntry | null>(null);
  const [progressById, setProgressById] = useState<
    Record<string, DownloadProgress>
  >({});
  const [resultById, setResultById] = useState<Record<string, DownloadResult>>(
    {},
  );
  const [busyIds, setBusyIds] = useState<Record<string, boolean>>({});
  const [error, setError] = useState<string | null>(null);
  const appUpdater = useAppUpdater();

  const loadTranslations = async () => {
    const list = await window.updater.getTranslations();
    setTranslations(list);
  };

  useEffect(() => {
    loadTranslations();
  }, []);

  useEffect(() => {
    const offProgress = window.updater.onDownloadProgress((payload) => {
      setProgressById((prev) => ({ ...prev, [payload.id]: payload }));
    });

    const offComplete = window.updater.onDownloadComplete((payload) => {
      setResultById((prev) => ({ ...prev, [payload.id]: payload }));
      loadTranslations();
    });

    return () => {
      offProgress?.();
      offComplete?.();
    };
  }, []);

  const filtered = useMemo(() => {
    const lowered = query.trim().toLowerCase();
    return translations
      .filter((item) => {
        if (!lowered) return true;
        return (
          item.Title.toLowerCase().includes(lowered) ||
          item.MetaInfo?.toLowerCase().includes(lowered)
        );
      })
      .sort((a, b) => {
        const favDelta = Number(!!b.PinnedState) - Number(!!a.PinnedState);
        if (favDelta !== 0) return favDelta;
        return a.Title.localeCompare(b.Title, "uk-UA");
      });
  }, [translations, query]);

  const handleSubmit = async (input: TranslationInput) => {
    if (editing) {
      await window.updater.updateTranslation(editing.Id, input);
    } else {
      await window.updater.addTranslation(input);
    }
    setModalOpen(false);
    setEditing(null);
    await loadTranslations();
  };

  const confirmRemove = async () => {
    if (!removing) return;
    const { Id } = removing;
    setRemoving(null);
    await window.updater.removeTranslation(Id);
    await loadTranslations();
  };

  const handleFavorite = async (id: string) => {
    await window.updater.toggleFavorite(id);
    await loadTranslations();
  };

  const handleUpdate = async (id: string) => {
    setError(null);
    setBusyIds((prev) => ({ ...prev, [id]: true }));
    try {
      const result = await window.updater.downloadAndInstall(id);
      setResultById((prev) => ({ ...prev, [id]: result }));
    } catch (err) {
      const message =
        err instanceof Error ? err.message : "Не вдалося завантажити.";
      setError(message);
    } finally {
      setBusyIds((prev) => ({ ...prev, [id]: false }));
    }
  };

  const handleRunExe = async (filePath: string) => {
    await window.updater.runExe(filePath);
  };

  const handleOpenGuide = async (url: string) => {
    const ok = await window.updater.openExternal(url);
    if (!ok) {
      setError("Не вдалося відкрити посібник.");
    }
  };

  const handleExtras = async () => {
    await window.updater.openStoreFolder();
  };

  const handleEdit = (item: TranslationEntry) => {
    setEditing(item);
    setModalOpen(true);
  };

  return (
    <div className="app">
      <TopBar
        onRefresh={loadTranslations}
        onAdd={() => {
          setEditing(null);
          setModalOpen(true);
        }}
        onExtras={handleExtras}
      />

      <SearchRow
        query={query}
        count={filtered.length}
        onQueryChange={setQuery}
      />

      <UpdateBanner
        state={appUpdater.state}
        onInstall={appUpdater.install}
        onDismiss={appUpdater.dismiss}
      />

      {error && <ErrorBanner message={error} />}

      <TranslationList
        items={filtered}
        progressById={progressById}
        resultById={resultById}
        busyIds={busyIds}
        onFavorite={handleFavorite}
        onEdit={handleEdit}
        onRemove={(id) =>
          setRemoving(translations.find((t) => t.Id === id) ?? null)
        }
        onUpdate={handleUpdate}
        onRunExe={handleRunExe}
        onOpenPath={(path) => window.updater.openPath(path)}
        onOpenGuide={handleOpenGuide}
      />

      <ConfirmModal
        open={removing !== null}
        title="Видалити переклад?"
        message={
          removing
            ? `«${removing.Title}» буде прибрано зі списку. Файли гри не змінюються.`
            : ""
        }
        onConfirm={confirmRemove}
        onCancel={() => setRemoving(null)}
      />

      <AddTranslationModal
        open={modalOpen}
        mode={editing ? "edit" : "create"}
        initial={editing ? toInput(editing) : null}
        onClose={() => {
          setModalOpen(false);
          setEditing(null);
        }}
        onSubmit={handleSubmit}
      />
    </div>
  );
}
