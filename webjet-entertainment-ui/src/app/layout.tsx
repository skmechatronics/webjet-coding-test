// app/layout.tsx
import './globals.css';
import { ReactNode } from 'react';

export const metadata = {
  title: 'WebJet Entertainment System',
  description: 'Movie metadata comparison UI',
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en" className="h-full">
      <body className="flex flex-col min-h-screen h-full bg-gray-100 text-gray-900">
        <header className="border-b bg-red-700 text-center text-gray-100">
          <div className="max-w-6xl mx-auto px-4 py-4 flex justify-center">
            <h1 className="text-xl font-semibold tracking-tight">
              WebJet Entertainment System
            </h1>
          </div>
        </header>

        <main className="flex-grow p-10 max-w-6xl mx-auto w-full">
          {children}
        </main>
      </body>
    </html>
  );
}