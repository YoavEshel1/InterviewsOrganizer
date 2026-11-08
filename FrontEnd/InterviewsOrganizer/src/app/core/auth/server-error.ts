import { HttpErrorResponse } from '@angular/common/http';

/** Extracts a user-facing message from an API failure; the API returns error text as a plain string body. */
export function getServerErrorMessage(error: unknown, fallback: string): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) {
      return 'Unable to reach the server. Please try again later.';
    }
    if (typeof error.error === 'string' && error.error.trim()) {
      return error.error;
    }
  }
  return fallback;
}
