// components/movie-metadata/PosterImage.tsx
'use client';

import Image from 'next/image';
import { useState } from 'react';

type PosterImageProps = {
  src: string;
  alt: string;
};

export default function PosterImage({ src, alt }: PosterImageProps) {
  const [imgSrc, setImgSrc] = useState(src);

  return (
    <div className="relative w-1/2 flex-shrink-0" style={{ aspectRatio: '2 / 3' }}>
      <Image
        src={imgSrc}
        alt={alt}
        fill
        className="rounded shadow-md object-cover"
        sizes="(max-width: 768px) 100vw, 300px"
        onError={() => setImgSrc('/images/default-poster.png')}
      />      
    </div>
  );
}
