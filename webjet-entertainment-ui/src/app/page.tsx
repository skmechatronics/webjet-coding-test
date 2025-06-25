'use client';

import { useEffect, useState } from 'react';
import MoviesCollection from './components/movies-collection';
import LoadingSpinner from './components/loading-spinner';

type Movie = {
  title: string;
  year: number;
  posterUrl: string;
  availableSources: string[];
};

export default function Page() {
  const [movies, setMovies] = useState<Movie[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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
            setLoading(false);
          }
        }, delay);
      } catch (err) {
        if (mounted) {
          setError((err as Error).message);
          setLoading(false);
        }
      }
    };

    fetchMovies();

    return () => {
      mounted = false;
    };
  }, []);

  if (loading) return <LoadingSpinner />;
  if (error) return <div className="text-center p-10 text-red-600">Error: {error}</div>;

  return (
    <main className="p-10 max-w-7.5xl mx-auto min-h-screen">
      <MoviesCollection movies={movies} />
    </main>
  );
}
