import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthService } from '../services/auth.service';

/**
 * Turns failed responses into something the user can read.
 *
 * 401 means the token is missing, expired or rejected: clear the session and send
 * the user to the login screen. Everything else surfaces as a snack bar and is
 * re-thrown so the calling component can still react.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Login and register answer bad credentials with 401 too. Treating those as an
      // expired session would bounce the user back to the login page with no message.
      if (error.status === 401 && !isCredentialRequest(req.url)) {
        authService.logout();
        router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
      } else {
        snackBar.open(extractMessage(error), 'Dismiss', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom'
        });
      }

      return throwError(() => error);
    })
  );
};

/** True for the endpoints where a 401 means "wrong credentials", not "session over". */
function isCredentialRequest(url: string): boolean {
  return url.includes('/auth/login') || url.includes('/auth/register');
}

/**
 * The API speaks two error shapes: `{ message }` from the exception middleware and
 * ValidationProblemDetails (`{ errors: { Field: [...] } }`) from model validation.
 */
function extractMessage(error: HttpErrorResponse): string {
  // status 0 means the request never landed: server down, or blocked by CORS or an
  // untrusted dev certificate.
  if (error.status === 0) {
    return 'Unable to reach the server. Check that the API is running.';
  }

  const body = error.error;

  // Blob bodies come back from responseType:'blob' calls such as the Excel export;
  // reading one is async, so fall back to a generic message.
  if (body instanceof Blob) {
    return `Request failed (${error.status}).`;
  }

  if (body && typeof body === 'object') {
    if (typeof body.message === 'string' && body.message) {
      return body.message;
    }

    if (body.errors && typeof body.errors === 'object') {
      const messages = Object.values(body.errors as Record<string, string[]>).flat();
      if (messages.length > 0) {
        return messages.join(' ');
      }
    }

    if (typeof body.title === 'string' && body.title) {
      return body.title;
    }
  }

  if (typeof body === 'string' && body) {
    return body;
  }

  return error.message || `Request failed (${error.status}).`;
}
