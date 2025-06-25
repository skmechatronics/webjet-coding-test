type TitleProps = {
  title: string;
  year: number;
};

export default function Title({ title, year }: TitleProps) {
  return (
    <div className="w-full px-1">
      <div className="text-base font-bold text-gray-900 leading-tight break-words whitespace-normal h-12 overflow-hidden">
        {title}
      </div>
       <div className="mt-2 text-sm font-bold text-white bg-red-700 border-b-4 border-red-900 rounded-md px-2 py-0.5  ">
         {year}
       </div>
    </div>
  );
}
