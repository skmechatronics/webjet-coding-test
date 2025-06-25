// components/movie-metadata/MetadataTabs.tsx
'use client';

import { useState } from 'react';
import Page1 from '@/app/components/movie-metadata/page1'
import Page2 from '@/app/components/movie-metadata/page2';
import type { MovieMetadataModel } from '@/app/components/movie-metadata';

type MetadataTabsProps = {
  metadata: MovieMetadataModel;
};

export default function MetadataTabs({ metadata }: MetadataTabsProps) {
  const [currentPage, setCurrentPage] = useState(1);

  return (
    <div>
      <div className="flex space-x-4 mb-6">
        {[1, 2].map((pageNum) => (
          <button
            key={pageNum}
            onClick={() => setCurrentPage(pageNum)}
            className={`px-4 py-2 rounded cursor-pointer ${
              currentPage === pageNum
                ? 'bg-red-700 text-white'
                : 'bg-white border border-gray-300 hover:bg-gray-100'
            }`}
            aria-pressed={currentPage === pageNum}
            type="button"
          >
            {pageNum === 1 ? 'General Info' : 'Cast & Awards'}
          </button>
        ))}
      </div>

      {currentPage === 1 && <Page1 metadata={metadata} />}
      {currentPage === 2 && <Page2 metadata={metadata} />}
    </div>
  );
}
