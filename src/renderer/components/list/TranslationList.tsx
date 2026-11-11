import { AnimatePresence } from "framer-motion";
import type {
  DownloadProgress,
  DownloadResult,
  TranslationEntry,
} from "../../../shared/types";
import EmptyState from "./EmptyState";
import TranslationCard from "./TranslationCard";

type Props = {
  items: TranslationEntry[];
  progressById: Record<string, DownloadProgress>;
  resultById: Record<string, DownloadResult>;
  busyIds: Record<string, boolean>;
  onFavorite: (id: string) => void;
  onEdit: (item: TranslationEntry) => void;
  onRemove: (id: string) => void;
  onUpdate: (id: string) => void;
  onRunExe: (filePath: string) => void;
  onOpenPath: (path: string) => void;
  onOpenGuide: (url: string) => void;
};

export default function TranslationList({
  items,
  progressById,
  resultById,
  busyIds,
  onFavorite,
  onEdit,
  onRemove,
  onUpdate,
  onRunExe,
  onOpenPath,
  onOpenGuide,
}: Props) {
  return (
    <section className="list">
      <AnimatePresence>
        {items.map((item) => (
          <TranslationCard
            key={item.Id}
            item={item}
            progress={progressById[item.Id]}
            result={resultById[item.Id]}
            busy={busyIds[item.Id]}
            onFavorite={onFavorite}
            onEdit={onEdit}
            onRemove={onRemove}
            onUpdate={onUpdate}
            onRunExe={onRunExe}
            onOpenPath={onOpenPath}
            onOpenGuide={onOpenGuide}
          />
        ))}
      </AnimatePresence>

      {items.length === 0 && <EmptyState />}
    </section>
  );
}
