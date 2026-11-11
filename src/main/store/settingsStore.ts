import Store from 'electron-store';
import { randomUUID } from 'crypto';
import type { SettingsStore, TranslationEntry, TranslationInput } from '../../shared/types';
import { resolveSteamMediaForPath } from '../services/steamMedia';

const store = new Store<SettingsStore>({
  name: 'settings',
  defaults: { List: [] }
});

export const getStorePath = () => store.path;

export const getTranslations = () => store.get('List', []);

export const setTranslations = (next: TranslationEntry[]) => {
  store.set('List', next);
  return next;
};

export const addTranslation = async (input: TranslationInput) => {
  const translation: TranslationEntry = {
    Id: randomUUID(),
    Title: input.Title.trim(),
    Url: input.Url.trim(),
    GuideUrl: input.GuideUrl?.trim() || '',
    Path: input.Path.trim(),
    Icon: input.Icon?.trim() || undefined,
    Banner: input.Banner?.trim() || undefined,
    MetaInfo: input.MetaInfo?.trim() || null,
    AddToList: !!input.AddToList,
    PinnedState: !!input.PinnedState,
    LastUpdate: undefined
  };

  if (!translation.Icon || !translation.Banner) {
    const media = await resolveSteamMediaForPath(translation.Path);
    if (media?.icon && !translation.Icon) {
      translation.Icon = media.icon;
    }
    if (media?.banner && !translation.Banner) {
      translation.Banner = media.banner;
    }
  }
  const list = getTranslations();
  list.push(translation);
  setTranslations(list);
  return translation;
};

export const updateTranslation = (id: string, patch: Partial<TranslationEntry>) => {
  const list = getTranslations();
  const index = list.findIndex((item) => item.Id === id);
  if (index === -1) return null;
  list[index] = { ...list[index], ...patch };
  setTranslations(list);
  return list[index];
};

export const removeTranslation = (id: string) => {
  const list = getTranslations();
  const next = list.filter((item) => item.Id !== id);
  setTranslations(next);
  return next.length !== list.length;
};

export const toggleFavorite = (id: string) => {
  const list = getTranslations();
  const index = list.findIndex((item) => item.Id === id);
  if (index === -1) return null;
  list[index] = { ...list[index], PinnedState: !list[index].PinnedState };
  setTranslations(list);
  return list[index];
};
