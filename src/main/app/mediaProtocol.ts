import { protocol } from 'electron';

export const registerMediaScheme = () => {
  protocol.registerSchemesAsPrivileged([
    {
      scheme: 'media',
      privileges: {
        standard: true,
        secure: true,
        supportFetchAPI: true,
        corsEnabled: true
      }
    }
  ]);
};

export const registerMediaProtocol = () => {
  protocol.registerFileProtocol('media', (request, callback) => {
    try {
      const url = new URL(request.url);
      const decoded = decodeURIComponent(url.pathname);
      const filePath = process.platform === 'win32' ? decoded.replace(/^\/+/, '') : decoded;
      callback({ path: filePath });
    } catch (error) {
      console.error('[media] Failed to resolve path', error);
    }
  });
};
