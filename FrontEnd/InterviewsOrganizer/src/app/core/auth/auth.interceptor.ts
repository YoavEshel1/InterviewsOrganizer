import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

/** Endpoints that never need a Bearer token. */
const AUTH_ENDPOINTS = ['/api/auth/login', '/api/auth/register', '/api/auth/refresh'];

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (AUTH_ENDPOINTS.some(url => req.url.includes(url))) {
    return next(req);
  }

  const token = authService.getAccessToken();
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401) {
        return throwError(() => error);
      }

      // Access token expired — try a silent refresh using the HttpOnly refresh-token cookie.
      return authService.refreshToken().pipe(
        switchMap(res => {
          const retryReq = req.clone({
            setHeaders: { Authorization: `Bearer ${res.accessToken}` },
          });
          return next(retryReq);
        }),
        catchError(refreshError => {
          router.navigate(['/login']);
          return throwError(() => refreshError);
        })
      );
    })
  );
};
