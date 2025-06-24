type PosterProps = {
    src: string;
    alt: string;
  };

  export default function Poster({ src, alt }: PosterProps) {
    return (
      <img
        src={src}
        alt={alt}
        className="w-[200px] h-[300px] object-cover rounded shadow-md"
        onError={(e) => {
          e.currentTarget.onerror = null;
          e.currentTarget.src = '/images/default-poster.png';
        }}
      />
    );
  }