type TitleProps = {
    title: string;
    year: number;
  };

  export default function Title({ title, year }: TitleProps) {
    return (
      <>
        <div className="font-medium">{title}</div>
        <div className="text-gray-500">{year}</div>
      </>
    );
  }