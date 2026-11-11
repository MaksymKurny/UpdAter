export const formatDate = (value?: string) => {
  if (!value) return "Ніколи";
  return new Date(value).toLocaleDateString("uk-UA");
};
