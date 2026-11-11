import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Frontend лежить у src/renderer, збірка потрапляє в dist (її бере Tauri).
export default defineConfig({
  root: 'src/renderer',
  plugins: [react()],
  clearScreen: false,
  server: {
    port: 5173,
    strictPort: true
  },
  build: {
    outDir: '../../dist',
    emptyOutDir: true,
    target: 'chrome105'
  }
});
