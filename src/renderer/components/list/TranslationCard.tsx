import { motion } from "framer-motion";
import {
  BookOpen,
  Download,
  FolderOpen,
  Pencil,
  Play,
  Star,
  Trash2,
} from "lucide-react";
import type {
  DownloadProgress,
  DownloadResult,
  TranslationEntry,
} from "../../../shared/types";
import { formatDate } from "../../utils/formatters";
import { resolveImageUrl } from "../../utils/media";

type Props = {
  item: TranslationEntry;
  progress?: DownloadProgress;
  result?: DownloadResult;
  busy?: boolean;
  onFavorite: (id: string) => void;
  onEdit: (item: TranslationEntry) => void;
  onRemove: (id: string) => void;
  onUpdate: (id: string) => void;
  onRunExe: (filePath: string) => void;
  onOpenPath: (path: string) => void;
  onOpenGuide: (url: string) => void;
};

const coverStyle = {
  "--cover": "linear-gradient(135deg, #1f2b3a 10%, #302046 100%)",
} as React.CSSProperties;

export default function TranslationCard({
  item,
  progress,
  result,
  busy,
  onFavorite,
  onEdit,
  onRemove,
  onUpdate,
  onRunExe,
  onOpenPath,
  onOpenGuide,
}: Props) {
  const actionLabel = busy
    ? "Завантаження..."
    : item.LastUpdate
      ? "Оновити"
      : "Завантажити";
  const bannerUrl = resolveImageUrl(item.Banner);
  const iconUrl = resolveImageUrl(item.Icon);
  const guideUrl = item.GuideUrl?.trim();

  return (
    <motion.div
      className="card"
      style={coverStyle}
      layout
      initial={{ opacity: 0, y: 18 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: 20 }}
    >
      {bannerUrl && (
        <img
          className="card-banner"
          src={bannerUrl}
          alt=""
          loading="lazy"
          onError={(event) => {
            event.currentTarget.style.display = "none";
          }}
        />
      )}
      <div className="card-content">
        <div className="card-header">
          <div>
            <div className="title-row">
              {iconUrl && (
                <img
                  className="game-icon"
                  src={iconUrl}
                  alt=""
                  loading="lazy"
                  onError={(event) => {
                    event.currentTarget.style.display = "none";
                  }}
                />
              )}
              <h3>{item.Title}</h3>
            </div>
            <p className="meta">
              Останнє завантаження: {formatDate(item.LastUpdate)}
            </p>
          </div>
          <div className="card-icons">
            <button
              className={`icon-btn ${item.PinnedState ? "active" : ""}`}
              onClick={() => onFavorite(item.Id)}
              title="В обране"
            >
              <Star size={18} />
            </button>
            <button
              className="icon-btn"
              onClick={() => onEdit(item)}
              title="Редагувати"
            >
              <Pencil size={18} />
            </button>
            <button
              className="icon-btn"
              onClick={() => onRemove(item.Id)}
              title="Видалити"
            >
              <Trash2 size={18} />
            </button>
          </div>
        </div>

        <div className="actions">
          <button
            className="btn"
            onClick={() => onUpdate(item.Id)}
            disabled={busy}
          >
            <Download size={16} />
            {actionLabel}
          </button>
          {result?.isExe && (
            <button
              className="btn ghost"
              onClick={() => onRunExe(result.filePath)}
            >
              <Play size={16} />
              Запустити
            </button>
          )}
          <button
            className="btn ghost"
            onClick={() => onOpenPath(item.Path)}
            title="Відкрити папку гри"
          >
            <FolderOpen size={16} />
            Відкрити папку
          </button>
          {guideUrl && (
            <button
              className="btn ghost"
              onClick={() => onOpenGuide(guideUrl)}
              title="Відкрити посібник"
            >
              <BookOpen size={16} />
              Відкрити посібник
            </button>
          )}
        </div>

        <div className="progress">
          <div
            className="progress-bar"
            style={{ width: `${progress?.percent ?? 0}%` }}
          ></div>
        </div>

        {progress && (
          <div className="progress-text">
            {progress.percent ? `${progress.percent}%` : "Завантаження..."}
          </div>
        )}
      </div>
    </motion.div>
  );
}
