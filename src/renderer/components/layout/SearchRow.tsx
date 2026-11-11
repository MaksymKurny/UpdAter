import { Search } from "lucide-react";

type Props = {
  query: string;
  count: number;
  onQueryChange: (value: string) => void;
};

export default function SearchRow({ query, count, onQueryChange }: Props) {
  return (
    <section className="search-row">
      <div className="search">
        <Search size={18} />
        <input
          value={query}
          onChange={(event) => onQueryChange(event.target.value)}
          placeholder="Пошук перекладів за назвою або нотатками"
        />
      </div>
      <div className="counter">{count} перекладів</div>
    </section>
  );
}
