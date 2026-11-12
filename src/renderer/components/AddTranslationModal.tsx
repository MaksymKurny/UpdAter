import { AnimatePresence, motion } from 'framer-motion';
import { FolderOpen, Image as ImageIcon, Star, X } from 'lucide-react';
import { useEffect, useState } from 'react';
import type { TranslationInput } from '../../shared/types';

const emptyForm: TranslationInput = {
  Title: '',
  Url: '',
  GuideUrl: '',
  Path: '',
  Icon: '',
  Banner: '',
  MetaInfo: '',
  AddToList: false,
  PinnedState: false
};

type Props = {
  open: boolean;
  mode: 'create' | 'edit';
  initial?: TranslationInput | null;
  onClose: () => void;
  onSubmit: (input: TranslationInput) => void;
};

export default function AddTranslationModal({ open, mode, initial, onClose, onSubmit }: Props) {
  const [form, setForm] = useState<TranslationInput>(emptyForm);
  const [error, setError] = useState('');

  useEffect(() => {
    if (open) {
      setForm(initial ? { ...emptyForm, ...initial } : emptyForm);
      setError('');
    }
  }, [open, initial]);

  const update = (patch: Partial<TranslationInput>) => {
    setForm((prev) => ({ ...prev, ...patch }));
  };

  const handleChooseFolder = async () => {
    const path = await window.updater.selectFolder();
    if (path) update({ Path: path });
  };

  const handleChooseImage = async (field: 'Banner' | 'Icon') => {
    const path = await window.updater.selectImage();
    if (path) update({ [field]: path });
  };

  const handleSubmit = () => {
    if (!form.Title.trim() || !form.Url.trim() || !form.Path.trim()) {
      setError('Заповніть назву, посилання та шлях встановлення.');
      return;
    }
    onSubmit({
      ...form,
      Title: form.Title.trim(),
      Url: form.Url.trim(),
      Path: form.Path.trim(),
      GuideUrl: form.GuideUrl?.trim() || '',
      Icon: form.Icon?.trim() || '',
      Banner: form.Banner?.trim() || '',
      MetaInfo: form.MetaInfo?.trim() || null
    });
  };

  return (
    <AnimatePresence>
      {open && (
        <motion.div
          className="modal-backdrop"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
        >
          <motion.div
            className="modal"
            initial={{ opacity: 0, y: 20, scale: 0.98 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: 20, scale: 0.98 }}
          >
            <div className="modal-header">
              <div>
                <h2>{mode === 'edit' ? 'Редагувати переклад' : 'Додати переклад'}</h2>
                <p>Працює з GitHub, Google Drive та прямими посиланнями.</p>
              </div>
              <button className="icon-btn" onClick={onClose} aria-label="Закрити">
                <X size={18} />
              </button>
            </div>

            <div className="form-grid">
              <label>
                Назва гри
                <input
                  value={form.Title}
                  onChange={(event) => update({ Title: event.target.value })}
                  placeholder="No Man's Sky"
                />
              </label>
              <label>
                Посилання на файл
                <input
                  value={form.Url}
                  onChange={(event) => update({ Url: event.target.value })}
                  placeholder="https://github.com/... або https://drive.google.com/..."
                />
              </label>
              <label className="span-2">
                Шлях встановлення
                <div className="inline-input">
                  <input
                    value={form.Path}
                    onChange={(event) => update({ Path: event.target.value })}
                    placeholder="C:\\Games\\NoMansSky"
                  />
                  <button className="btn ghost" onClick={handleChooseFolder} type="button">
                    <FolderOpen size={16} />
                    Обрати
                  </button>
                </div>
              </label>
              <label>
                Банер (необов'язково)
                <div className="inline-input">
                  <input
                    value={form.Banner}
                    onChange={(event) => update({ Banner: event.target.value })}
                    placeholder="https://.../banner.jpg"
                  />
                  <button
                    className="btn ghost"
                    onClick={() => handleChooseImage('Banner')}
                    type="button"
                    title="Обрати файл зображення"
                  >
                    <ImageIcon size={16} />
                    Обрати
                  </button>
                </div>
              </label>
              <label>
                Іконка (необов'язково)
                <div className="inline-input">
                  <input
                    value={form.Icon}
                    onChange={(event) => update({ Icon: event.target.value })}
                    placeholder="https://.../icon.jpg"
                  />
                  <button
                    className="btn ghost"
                    onClick={() => handleChooseImage('Icon')}
                    type="button"
                    title="Обрати файл зображення"
                  >
                    <ImageIcon size={16} />
                    Обрати
                  </button>
                </div>
              </label>
              <label>
                Посилання на гайд (необов'язково)
                <input
                  value={form.GuideUrl}
                  onChange={(event) => update({ GuideUrl: event.target.value })}
                  placeholder="https://..."
                />
              </label>
              <label>
                Нотатки (необов'язково)
                <input
                  value={form.MetaInfo ?? ''}
                  onChange={(event) => update({ MetaInfo: event.target.value })}
                  placeholder="Напр., версія 1.2"
                />
              </label>
            </div>

            <div className="modal-footer">
              <button
                className={`btn ghost ${form.PinnedState ? 'active' : ''}`}
                onClick={() => update({ PinnedState: !form.PinnedState })}
              >
                <Star size={16} />
                {form.PinnedState ? 'В обраному' : 'Додати в обране'}
              </button>
              <div className="modal-actions">
                {error && <span className="error-text">{error}</span>}
                <button className="btn ghost" onClick={onClose}>
                  Скасувати
                </button>
                <button className="btn primary" onClick={handleSubmit}>
                  {mode === 'edit' ? 'Зберегти' : 'Додати'}
                </button>
              </div>
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
}
