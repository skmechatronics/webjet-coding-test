'use client';

import { useEffect, useState, useRef } from 'react';
import LoadingSpinner from './loading-spinner';
import PriceComparison from "./movie-metadata/price-comparison";
import CsvBulletedList from "./movie-metadata/csv-bulleted-list";

type MovieMetadataProps = {
  title: string | null;
};

type SourcePrice = {
  source: string;
  price: number;
};

type MovieMetadataModel = {
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
      className="mt-16 outline-none"
      aria-live="polite"
      aria-atomic="true"
    >
      {loading && <LoadingSpinner />}

      {error && (
        <div className="text-red-600 font-semibold text-center">{error}</div>
      )}

      {metadata && (
        <div className="bg-red-100 border-red-900 border-t-[10px] p-6 rounded-lg shadow-md max-w-7xl mx-auto flex gap-8">
          {/* Poster left side */}
          {metadata.posterUrl && (
            <img
              src={metadata.posterUrl}
              alt={`${metadata.title} poster`}
              onError={(e) => {
                e.currentTarget.onerror = null;
                e.currentTarget.src = '/images/default-poster.png';
              }}
              className="rounded shadow-md w-1/2 object-cover flex-shrink-0"
            />
          )}

          {/* Details right side */}
          <div className="flex-grow w-1/2">
            <h3 className="text-4xl font-extrabold mb-6">
              {metadata.title} <span className="text-red-900 text-3xl">({metadata.year})</span>
            </h3>

            <div className="flex flex-col">
              <div className="whitespace-pre-wrap border-x-4 border-red-800 p-4 rounded text-lg text-gray-800 font-medium italic mt-2 mb-6">
                {metadata.plot || 'N/A'}
              </div>
            </div>


            <dl className="grid grid-cols-1 gap-y-3 text-gray-700">

              <div className="flex justify-between">
                <dt className="font-semibold">Rated:</dt>
                <dd>{metadata.rated || 'N/A'}</dd>
              </div>

              <div className="flex justify-between">
                <dt className="font-semibold">Released:</dt>
                <dd>{new Date(metadata.released).toLocaleDateString()}</dd>
              </div>

              <div className="flex justify-between">
                <dt className="font-semibold">Runtime:</dt>
                <dd>{metadata.runtime || 'N/A'}</dd>
              </div>

              <div className="flex justify-between">
                <dt className="font-semibold">Genre:</dt>
                <dd>{metadata.genre || 'N/A'}</dd>
              </div>

              <div className="flex justify-between">
                <dt className="font-semibold">Director:</dt>
                <dd>{metadata.director || 'N/A'}</dd>
              </div>

              <div className="flex justify-between">
                <dt className="font-semibold">Language:</dt>
                <dd>{metadata.language || 'N/A'}</dd>
              </div>

              <div className="flex justify-between">
                <dt className="font-semibold">Country:</dt>
                <dd>{metadata.country || 'N/A'}</dd>
              </div>

              <div className="flex justify-between">
                <dt className="font-semibold">Metascore:</dt>
                <dd>{metadata.metascore > 0 ? metadata.metascore : 'N/A'}</dd>
              </div>

              <div className="flex justify-between">
                <dt className="font-semibold">Rating:</dt>
                <dd>{metadata.rating > 0 ? metadata.rating.toFixed(1) : 'N/A'}</dd>
              </div>

              <div className="flex justify-between">
                <dt className="font-semibold">Votes:</dt>
                <dd>{metadata.votes > 0 ? metadata.votes.toLocaleString() : 'N/A'}</dd>
              </div>


              <div className="justify-between inline-block">
                <dt className="font-semibold">Writers:</dt>
                <dd><CsvBulletedList csv={metadata.writer} /></dd>
              </div>

              <div>
                <dt className="font-semibold">Actors:</dt>
                <dd><CsvBulletedList csv={metadata.actors} /></dd>
              </div>
              <div className="inline-block">
                <dt className="font-semibold">Awards:</dt>
                <dd>
                  <CsvBulletedList csv={metadata.awards} separator="." />
                </dd>
              </div>
            </dl>


            {metadata.sourcePrices && (
              <PriceComparison sourcePrices={metadata.sourcePrices} />
            )}
          </div>
        </div>
      )}
    </section>
  );
}
