'use client';

import { useEffect, useState, useRef } from 'react';
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

  // retry count and mounted refs to persist across retries
  const retryCount = useRef(0);
  const mounted = useRef(true);
  const timeoutId = useRef<NodeJS.Timeout>();

  useEffect(() => {
    mounted.current = true;

    const fetchMovies = async () => {
      try {
        const url = `${process.env.NEXT_PUBLIC_WEBJET_ENTERTAINMENT_API_URL}/api/movies/movie-collection`;
        const res = await fetch(url);
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
        const data = await res.json();
        const sorted = data.sort((a: Movie, b: Movie) => b.year - a.year);
        const MinimumRetriesBeforeExtend = 3;
        const RefreshNormal = 5000;    // 5 seconds
        const RefreshExtended = 15000; // 15 seconds



        if (!mounted.current) return;

        setMovies(sorted);
        setLoadingMovies(false);
        setErrorMovies(null);
        retryCount.current = 0; // reset retries on success
      } catch (err) {
        if (!mounted.current) return;

        setErrorMovies((err as Error).message);
        setLoadingMovies(false);

        // Calculate next retry delay
        retryCount.current++;
        const delay = retryCount.current < MinimumRetriesBeforeExtend
          ? RefreshNormal
          : RefreshExtended;        

        timeoutId.current = setTimeout(() => {
          if (mounted.current) {
            setLoadingMovies(true);
            fetchMovies();
          }
        }, delay);
      }
    };

    fetchMovies();

    return () => {
      mounted.current = false;
      if (timeoutId.current) clearTimeout(timeoutId.current);
    };
  }, []);

  if (loadingMovies) return <LoadingSpinner />;
  if (errorMovies) return <ErrorScreen showRetryMessage={true} message="Sorry, we’re having trouble loading the info at the moment." />;

  return (
    <main className="px-10 pb-10 max-w-7.5xl mx-auto min-h-screen">
      <MoviesCollection movies={movies} onSelectMovie={setSelectedTitle} />
      <MovieMetadata title={selectedTitle} />
    </main>
  );
}
