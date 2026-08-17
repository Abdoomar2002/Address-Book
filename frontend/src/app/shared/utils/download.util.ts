/**
 * Saves a Blob to the user's machine via a temporary object URL and anchor.
 *
 * The URL is revoked on the next tick rather than immediately: revoking while the
 * click is still being processed can cancel the download in some browsers.
 */
export function downloadBlob(blob: Blob, fileName: string): void {
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement('a');

  anchor.href = url;
  anchor.download = fileName;
  anchor.style.display = 'none';

  document.body.appendChild(anchor);
  anchor.click();
  anchor.remove();

  setTimeout(() => URL.revokeObjectURL(url));
}
