type ErrorScreenProps = {
  message: string;
  showRetryMessage?: boolean;
};

export default function ErrorScreen({ message, showRetryMessage = false }: ErrorScreenProps) {
  return (
    <main
      role="alert"
      aria-live="assertive"
      tabIndex={-1}
      className="flex flex-col items-center justify-center p-10 bg-red-50 text-red-700 focus:outline-none focus:ring-4 focus:ring-red-400"
    >
      <h1 className="text-3xl font-semibold mb-4">{message}</h1>
      {showRetryMessage && (
        <p className="text-xl italic">Trying again shortly...</p>
      )}
    </main>
  );
}
