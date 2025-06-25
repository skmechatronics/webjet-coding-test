'use client';

import { useEffect, useState, useRef } from 'react';
import LoadingSpinner from './loading-spinner';
import PriceComparison from './movie-metadata/price-comparison';
import MetadataTabs from './movie-metadata/metadata-tabs';
import MetadataTabContent from './movie-metadata/metadata-tab-content';
import PosterImage from './movie-metadata/poster-image';
import ErrorScreen from './error-screen'

type MovieMetadataProps = {
  title: string | null;
};

export type SourcePrice = {
  source: string;
  price: number;
};

export type MovieMetadataModel = {
  title: string;
  year: number;
  rated: string;
  released: string;
  runtime: string;
  genre: string;
  director: string;
  writer: string;
  actors: string;
  plot: string;
  language: string;
  country: string;
  awards: string;
  posterUrl: string;
  metascore: number;
  rating: number;
  votes: number;
  sourcePrices: SourcePrice[];
};

export default function MovieMetadata({ title }: MovieMetadataProps) {
  const [metadata, setMetadata] = useState<MovieMetadataModel | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);

  const metadataRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!title) {
      setMetadata(null);
      setError(null);
      setLoading(false);
      return;
    }

    let mounted = true;
    const start = Date.now();

    const fetchMetadata = async () => {
      setLoading(true);
      setError(null);
      setMetadata(null);

      try {
        const encodedTitle = encodeURIComponent(title);
        const url = `${process.env.NEXT_PUBLIC_WEBJET_ENTERTAINMENT_API_URL}/api/movies/${encodedTitle}/price-comparison`;
        const res = await fetch(url);
        if (!res.ok) {
          if (res.status === 404) throw new Error('No metadata found for this movie.');
          else throw new Error(`Error fetching metadata: ${res.status}`);
        }
        const data: MovieMetadataModel = await res.json();

        const elapsed = Date.now() - start;
        const delay = elapsed < 500 ? 500 - elapsed : 0;

        setTimeout(() => {
          if (mounted) {
            setMetadata(data);
            setLoading(false);
            setError(null);
            setPage(1); // reset to page 1 when metadata changes
            if (metadataRef.current) {
              metadataRef.current.scrollIntoView({ behavior: 'smooth', block: 'start' });
              metadataRef.current.focus();
            }
          }
        }, delay);
      } catch (err) {
        if (mounted) {
          setError((err as Error).message);
          setLoading(false);
        }
      }
    };

    fetchMetadata();

    return () => {
      mounted = false;
    };
  }, [title]);

  if (!title) return null;

  return (
    <section
      tabIndex={-1}
      ref={metadataRef}
      className="mt-16 outline-none max-w-7xl mx-auto"
      aria-live="polite"
      aria-atomic="true"
    >
      {loading && <LoadingSpinner />}
      {error && (
          <ErrorScreen className={"height-1/2"} showRetryMessage={false} message="Sorry we couldn't retrieve the pricing at this time. Please try again shortly." />
      )}

      {metadata && (
        <div className="bg-red-100 border-red-900 border-t-[10px] p-6 rounded-lg shadow-md flex gap-8">
          {/* Poster */}
          {metadata.posterUrl && (
            <PosterImage src={metadata.posterUrl} alt={`${metadata.title} poster`} />
          )}

          <div className="flex-grow w-1/2 flex flex-col">
            <h3 className="text-4xl font-extrabold mb-6">
              {metadata.title} <span className="text-red-900 text-3xl">({metadata.year})</span>
            </h3>

            <div className="whitespace-pre-wrap border-x-4 border-red-800 p-4 rounded text-lg text-gray-800 font-medium italic mb-6 flex-shrink-0">
              {metadata.plot || 'N/A'}
            </div>

            <div className="flex-grow min-h-[400px]">
              <MetadataTabs metadata={metadata} />
            </div>

            {metadata?.sourcePrices && (
              <div className="max-w-7xl mt-8 inline-block">
                <PriceComparison sourcePrices={metadata.sourcePrices} />
              </div>
            )}
          </div>
        </div>
      )}

    </section>
  );
}
