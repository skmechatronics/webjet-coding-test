'use client';

import { useEffect, useState } from 'react';
import Poster from './movies-collection/poster';
import Title from './movies-collection/title';
import Tags from './movies-collection/tags';

type Movie = {
  title: string;
  year: number;
  posterUrl: string;
  availableSources: string[];
};

export default function Home() {
  const [movies, setMovies] = useState<Movie[]>([]);

  useEffect(() => {
    const fetchMovies = async () => {
      try {
        const url = `${process.env.NEXT_PUBLIC_WEBJET_ENTERTAINMENT_API_URL}/api/movies/movie-collection`;
        const res = await fetch(url);
        const data = await res.json();
        const sorted = data.sort((a: Movie, b: Movie) => b.year - a.year);
        setMovies(sorted);
      } catch (err) {
        console.error('Failed to fetch movies:', err);
      }
    };

    fetchMovies();
  }, []);

  return (
    <div className="space-y-8">
      <h2 className="text-2xl font-semibold mb-4 text-center">Select your movie!</h2>
      <div className="flex scrollbar-custom overflow-x-auto space-x-6 pb-4 h-full items-start overflow-visible">
        {movies.map((movie, idx) => (
          <div key={idx} className="my-10 mx-5 max-w-[200px] flex-shrink-0 text-center text-sm transition-transform duration-200 hover:scale-110 hover:z-20">
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
