// app/layout.tsx
import './globals.css';
import { ReactNode } from 'react';

export const metadata = {
  title: 'WebJet Entertainment System',
  description: 'Movie metadata comparison UI',
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en">
      <body className="min-h-screen bg-gray-100 text-gray-900">
        <header className="border-b bg-white">
          <div className="max-w-6xl mx-auto px-4 py-4 flex justify-center">
            <h1 className="text-xl font-semibold tracking-tight">
              WebJet Entertainment System
            </h1>
          </div>
        </header>
        <main className="p-6 max-w-6xl mx-auto">{children}</main>
      </body>
    </html>
  );
}