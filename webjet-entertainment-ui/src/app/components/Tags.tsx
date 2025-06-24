type TagsProps = {
    sources: string[];
  };

  export default function Tags({ sources }: TagsProps) {
    return (
      <div className="mt-2 text-xs text-gray-500 text-center">
        <span className="block mb-1">Available on:</span>
        <div className="flex flex-wrap justify-center gap-2">
          {sources.map((source, i) => {
            const tagClasses =
              source === 'Cinemaworld'
                ? 'bg-green-600'
                : source === 'Filmworld'
                ? 'bg-orange-500'
                : 'bg-gray-500';

            return (
              <span
                key={i}
                className={`${tagClasses} text-white px-2 py-0.5 rounded-full text-xs font-semibold`}
              >
                {source}
              </span>
            );
          })}
        </div>
      </div>
    );
  }