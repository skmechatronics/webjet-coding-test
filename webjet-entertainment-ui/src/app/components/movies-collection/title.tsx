type TitleProps = {
  title: string;
  year: number;
};

export default function Title({ title, year }: TitleProps) {
  return (
    <div className="w-full px-1">
      <div className="text-sm font-semibold text-gray-800 leading-tight break-words whitespace-normal">
        {title}
      </div>
      <div className="text-gray-500 text-xs">{year}</div>
    </div>
  );
}
