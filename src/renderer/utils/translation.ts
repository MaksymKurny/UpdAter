import type { TranslationEntry, TranslationInput } from "../../shared/types";

export const toInput = (item: TranslationEntry): TranslationInput => ({
  Title: item.Title ?? "",
  Url: item.Url ?? "",
  GuideUrl: item.GuideUrl ?? "",
  Path: item.Path ?? "",
  Icon: item.Icon ?? "",
  Banner: item.Banner ?? "",
  MetaInfo: item.MetaInfo ?? "",
  AddToList: !!item.AddToList,
  PinnedState: !!item.PinnedState,
});
