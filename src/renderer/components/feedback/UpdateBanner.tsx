import { Download } from "lucide-react";
import type { AppUpdaterState } from "../../hooks/useAppUpdater";

type Props = {
  state: AppUpdaterState;
  onInstall: () => void;
  onDismiss: () => void;
};

export default function UpdateBanner({ state, onInstall, onDismiss }: Props) {
  if (state.status === "idle") return null;

  if (state.status === "error") {
    return (
      <div className="update-banner update-banner-error">
        <span>Не вдалося оновити програму: {state.error}</span>
        <button className="btn subtle" onClick={onDismiss}>
          Закрити
        </button>
      </div>
    );
  }

  const busy = state.status === "downloading" || state.status === "ready";

  return (
    <div className="update-banner">
      <span>
        <Download size={16} />
        Доступна нова версія UpdAter {state.version}
        {busy && (state.percent !== undefined ? ` — ${state.percent}%` : " — завантаження...")}
      </span>
      {!busy && (
        <div className="update-banner-actions">
          <button className="btn primary" onClick={onInstall}>
            Оновити й перезапустити
          </button>
          <button className="btn subtle" onClick={onDismiss}>
            Пізніше
          </button>
        </div>
      )}
    </div>
  );
}
