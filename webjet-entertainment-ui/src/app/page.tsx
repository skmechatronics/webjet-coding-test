'use client';

import { useEffect, useState } from 'react';
import LoadingSpinner from './components/loading-spinner';
import MoviesCollection from './components/movies-collection';
import ErrorScreen from './components/error-screen';
import MovieMetadata from './components/movie-metadata';

type Movie = {
  title: string;
  year: number;
  posterUrl: string;
  availableSources: string[];
};

export default function Page() {
  const [movies, setMovies] = useState<Movie[]>([]);
  const [loadingMovies, setLoadingMovies] = useState(true);
  const [errorMovies, setErrorMovies] = useState<string | null>(null);

  const [selectedTitle, setSelectedTitle] = useState<string | null>(null);

  useEffect(() => {
    let mounted = true;
    const start = Date.now();

    const fetchMovies = async () => {
      try {
        const url = `${process.env.NEXT_PUBLIC_WEBJET_ENTERTAINMENT_API_URL}/api/movies/movie-collection`;
        const res = await fetch(url);
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
        const data = await res.json();
        const sorted = data.sort((a: Movie, b: Movie) => b.year - a.year);
        const elapsed = Date.now() - start;
        const delay = elapsed < 500 ? 500 - elapsed : 0;
        setTimeout(() => {
          if (mounted) {
            setMovies(sorted);
            setLoadingMovies(false);
            setErrorMovies(null);
          }
        }, delay);
      } catch (err) {
        if (mounted) {
          setErrorMovies((err as Error).message);
          setLoadingMovies(false);
        }
      }
    };

    fetchMovies();

    return () => {
      mounted = false;
    };
  }, []);

  if (loadingMovies) return <LoadingSpinner />;
  if (errorMovies) return <ErrorScreen message={errorMovies} />;

  return (
    <main className="px-10 pb-10 max-w-7.5xl mx-auto min-h-screen">
      <MoviesCollection movies={movies} onSelectMovie={setSelectedTitle} />
      <MovieMetadata title={selectedTitle} />
    </main>
  );
}
