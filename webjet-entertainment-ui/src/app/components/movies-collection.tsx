import CarouselPoster from './movies-collection/carousel-poster';
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
  onSelectMovie: (title: string) => void;
};

export default function MoviesCollection({ movies, onSelectMovie }: MoviesCollectionProps) {
  return (
    <div className="space-y-8">
      <h2
        className="text-3xl mb-2 text-center"
        style={{ fontFamily: 'Impact,Charcoal,sans-serif' }}
      >
        NOW SHOWING
      </h2>
      <div
        className="flex bg-red-100/50 scrollbar-custom overflow-x-auto space-x-8 h-full items-start
                   backdrop-blur-md border border-red-100/30 rounded-lg shadow-lg"
      >
        {movies.map((movie, idx) => (
          <div
            key={idx}
            onClick={() => onSelectMovie(movie.title)}
            tabIndex={0}
            role="button"
            onKeyDown={(e) => {
              if (e.key === 'Enter' || e.key === ' ') onSelectMovie(movie.title);
            }}
            className="my-10 mx-5 max-w-[200px] flex-shrink-0 text-center text-sm transition-transform duration-200 hover:scale-110 hover:z-20 cursor-pointer focus:outline-none focus:ring-2 focus:ring-red-600"
          >
            <CarouselPoster src={movie.posterUrl} alt={movie.title} />
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
