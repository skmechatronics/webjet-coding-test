type TagsProps = {
    sources: string[];
  };

  function getTagClasses(source: string) {
    switch (source) {
      case 'Cinemaworld':
        return 'bg-green-600';
      case 'Filmworld':
        return 'bg-orange-500';
      default:
        return 'bg-gray-500';
    }
  }
  export default function Tags({ sources }: TagsProps) {
    return (
      <div className="mt-2">
        <span className="block mb-1 text-sm text-black text-center font-bold">Available on</span>
        <div className="flex flex-wrap justify-center gap-2">
          {sources.map((source, i) => (
            <span
              key={i}
              className={`${getTagClasses(source)} text-white px-2 py-0.5 rounded-full text-l font-semibold`}
            >
              {source}
            </span>
          ))}
        </div>
      </div>
    );
  }