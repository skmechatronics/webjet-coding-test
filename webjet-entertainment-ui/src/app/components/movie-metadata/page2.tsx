// components/movie-metadata/Page2.tsx
'use client';

import CsvBulletedList from './csv-bulleted-list';
import type { MovieMetadataModel } from './MovieMetadata';

type Page2Props = {
  metadata: MovieMetadataModel;
};

export default function Page2({ metadata }: Page2Props) {
  return (
    <dl className="grid grid-cols-1 gap-y-3 text-gray-700">
      <div>
        <dt className="font-semibold mb-1">Writers:</dt>
        <dd>
          <CsvBulletedList csv={metadata.writer} />
        </dd>
      </div>
      <div>
        <dt className="font-semibold mb-1">Actors:</dt>
        <dd>
          <CsvBulletedList csv={metadata.actors} />
        </dd>
      </div>
      <div>
        <dt className="font-semibold mb-1">Awards:</dt>
        <dd>
          <CsvBulletedList csv={metadata.awards} separator="." />
        </dd>
      </div>
    </dl>
  );
}
