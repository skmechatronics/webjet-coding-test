// components/movie-metadata/Page1.tsx
'use client';

import type { MovieMetadataModel } from './MovieMetadata';

type Page1Props = {
  metadata: MovieMetadataModel;
};

export default function Page1({ metadata }: Page1Props) {
  return (
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
    </dl>
  );
}
