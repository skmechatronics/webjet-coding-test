// app/layout.tsx
import './globals.css';
import { ReactNode } from 'react';
import AppHeader from './components/app-header'

export const metadata = {
  title: 'WebJet Entertainment System',
  description: 'Movie metadata comparison UI',
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en" className="h-full">
      <body className="flex flex-col min-h-screen h-full bg-gray-100 text-gray-900">
        <AppHeader/>
        <main className="flex-grow p-5 max-w-7xl mx-auto">
          {children}
        </main>
      </body>
    </html>
  );
}