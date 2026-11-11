export type TranslationEntry = {
  Id: string;
  Title: string;
  Url: string;
  GuideUrl?: string;
  Path: string;
  Icon?: string;
  Banner?: string;
  LastUpdate?: string;
  AddToList?: boolean;
  PinnedState?: boolean;
  MetaInfo?: string | null;
};

export type SettingsStore = {
  List: TranslationEntry[];
};

export type TranslationInput = Omit<TranslationEntry, 'Id' | 'LastUpdate'>;

export type DownloadProgress = {
  id: string;
  received: number;
  total?: number;
  percent?: number;
};

export type DownloadResult = {
  id: string;
  filePath: string;
  fileName: string;
  isExe: boolean;
  extracted: boolean;
  copied: boolean;
};
