type PriceComparisonProps = {
    sourcePrices: { source: string; price: number }[];
  };

  export default function PriceComparison({ sourcePrices }: PriceComparisonProps) {
    if (!sourcePrices || sourcePrices.length === 0) return null;

    return (
      <div className="mt-8 p-4 bg-white rounded border-x-4 border-red-800">
        <h4 className="text-lg font-semibold mb-2">Price Comparison</h4>
        <ul className="space-y-1">
          {sourcePrices.map(({ source, price }, i) => (
            <li
              key={source}
              className={`flex justify-between px-2 py-1 rounded ${
                i === 0 ? 'bg-green-100 font-bold' : ''
              }`}
            >
              <span>{source}</span>
              <span>${price.toFixed(2)}</span>
            </li>
          ))}
        </ul>
        <p className="mt-2 text-sm text-green-700 font-semibold">
          Cheapest: {sourcePrices[0].source} at ${sourcePrices[0].price.toFixed(2)}
        </p>
      </div>
    );
  }
