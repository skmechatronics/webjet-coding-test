import CsvBulletedList from "./csv-bulleted-list";

type MetadataTabContentProps = {
  page: number;
  metadata: MovieMetadataModel; // Make sure to import or define this type where used
};

export default function MetadataTabContent({ page, metadata }: MetadataTabContentProps) {
  if (page === 1) {
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
      </dl>
    );
  }

  if (page === 2) {
    return (
      <>
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
      </>
    );
  }

  return null;
}
