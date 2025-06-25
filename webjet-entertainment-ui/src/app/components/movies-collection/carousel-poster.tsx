'use client';

import Image from 'next/image';
import { useState } from 'react';

type CarouselPosterProps = {
  src: string;
  alt: string;
};

export default function CarouselPoster({ src, alt }: CarouselPosterProps) {
  const [imgSrc, setImgSrc] = useState(src);

  return (
    <div className="relative w-[200px] h-[300px] flex-shrink-0">
      <Image
        src={imgSrc}
        alt={alt}
        fill
        className="rounded shadow-md object-cover"
        sizes="200px"
        onError={() => setImgSrc('/images/default-poster.png')}
      />
    </div>
  );
}
