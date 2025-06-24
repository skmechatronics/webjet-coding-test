'use client';

import { useEffect, useState } from 'react';
import Poster from './components/Poster';
import Title from './components/Title';
import Tags from './components/Tags';

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
        setMovies(data);
      } catch (err) {
        console.error('Failed to fetch movies:', err);
      }
    };

    fetchMovies();
  }, []);

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold mb-4">Now Showing</h2>
      <div className="flex overflow-x-auto space-x-6 pb-4">
        {movies.map((movie, idx) => (
          <div key={idx} className="min-w-[200px] flex-shrink-0 text-center text-sm">
            <Poster src={movie.posterUrl} alt={movie.title} />
            <div className="mt-2">
              <Title title={movie.title} year={movie.year} />
              <Tags sources={movie.availableSources} />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
