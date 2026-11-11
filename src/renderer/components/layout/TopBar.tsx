import { MoreHorizontal, Plus, RefreshCw } from "lucide-react";

type Props = {
  onRefresh: () => void;
  onAdd: () => void;
  onExtras: () => void;
};

export default function TopBar({ onRefresh, onAdd, onExtras }: Props) {
  return (
    <header className="top-bar">
      <div>
        <div className="brand">
          <span style={{ color: "blue" }}>U</span>pd
          <span style={{ color: "yellow" }}>A</span>ter
        </div>
        <p className="subtitle">Автоматичні оновлення українізаторів для ігор</p>
      </div>
      <div className="top-actions">
        <button className="btn ghost" onClick={onRefresh}>
          <RefreshCw size={16} />
          Оновити список
        </button>
        <button className="btn primary" onClick={onAdd}>
          <Plus size={16} />
          Додати ще
        </button>
        <button className="btn subtle" onClick={onExtras}>
          <MoreHorizontal size={16} />
          Додатково
        </button>
      </div>
    </header>
  );
}
