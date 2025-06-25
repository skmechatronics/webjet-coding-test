import Poster from './movies-collection/poster';
import Title from './movies-collection/title';
import Tags from './movies-collection/tags';

type Movie = {
  title: string;
  year: number;
  posterUrl: string;
  availableSources: string[];
};

type MoviesCollectionProps = {
  movies: Movie[];
};

export default function MoviesCollection({ movies }: MoviesCollectionProps) {
  return (
    <div className="space-y-8">
      <h2 className="text-xl font-cursive mb-2 text-center">NOW SHOWING</h2>
      <div className="flex bg-red-100 scrollbar-custom overflow-x-auto space-x-8 h-full items-start">
        {movies.map((movie, idx) => (
          <div
            key={idx}
            className="my-10 mx-5 max-w-[200px] flex-shrink-0 text-center text-sm transition-transform duration-200 hover:scale-110 hover:z-20"
          >
            <Poster src={movie.posterUrl} alt={movie.title} />
            <div className="mt-4">
              <Title title={movie.title} year={movie.year} />
              <Tags sources={movie.availableSources} />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
