'use client';

import { useEffect, useState } from 'react';
import LoadingScreen from './components/loading-spinner';
import MoviesCollection from './components/movies-collection';
import ErrorScreen from './components/error-screen';

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
  const [retryCount, setRetryCount] = useState(0);

  const InitialRefreshPeriod = 5000;
  const MaxRefreshPeriod = 15000;
  const ExcessiveRetryCount = 3;
  const fetchMovies = async () => {
    try {
      const url = `${process.env.NEXT_PUBLIC_WEBJET_ENTERTAINMENT_API_URL}/api/movies/movie-collection`;
      const res = await fetch(url);
      if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
      const data = await res.json();
      const sorted = data.sort((a: Movie, b: Movie) => b.year - a.year);
      setMovies(sorted);
      setError(null);
      setRetryCount(0); // reset retries on success
    } catch (err) {
      setError((err as Error).message);
      setRetryCount((count) => count + 1);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMovies();
  }, []);

  useEffect(() => {
    if (!error) return;

    const delay = retryCount > ExcessiveRetryCount ? MaxRefreshPeriod : InitialRefreshPeriod;

    const interval = setInterval(() => {
      setLoading(true);
      fetchMovies();
    }, delay);

    return () => clearInterval(interval);
  }, [error, retryCount]);

  if (loading) return <LoadingScreen />;

  if (error) return <ErrorScreen message="Unfortunately we are experiencing technical difficulties"/>;

  return (
    <main className="pl-10 pr-10 pb-10 max-w-7xl mx-auto min-h-screen">
      <MoviesCollection movies={movies} />
    </main>
  );
}
