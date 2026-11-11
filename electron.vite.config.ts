import { defineConfig } from 'electron-vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  main: {
    entry: 'src/main/index.ts',
    build: {
      rollupOptions: {
        external: ['7zip-bin-full']
      }
    }
  },
  preload: {
    input: {
      index: 'src/preload/index.ts'
    }
  },
  renderer: {
    root: 'src/renderer',
    plugins: [react()],
    build: {
      outDir: '../../out/renderer',
      emptyOutDir: true
    }
  }
});
