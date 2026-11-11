declare module 'node-7z' {
  import type { EventEmitter } from 'events';

  export function extractFull(
    archivePath: string,
    destination: string,
    options?: Record<string, unknown>
  ): EventEmitter;
}
