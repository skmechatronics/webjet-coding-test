type CsvBulletedListProps = {
  csv: string;
  separator?: string;
};

export default function CsvBulletedList({ csv, separator = ',' }: CsvBulletedListProps) {
  if (!csv) return null;

  const items = csv.split(separator)
                  .filter(item => item.trim() !== '')
                  .map(item => item.trim());

  return (
    <ul className="list-disc list-inside pl-6">
      {items.map((item, idx) => (
        <li key={idx}>{item}</li>
      ))}
    </ul>
  );
}
