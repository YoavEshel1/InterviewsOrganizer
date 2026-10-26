import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, catchError, finalize, firstValueFrom, tap, throwError } from 'rxjs';

export interface LoginResponse {
  token: string;
}

export interface RefreshResponse {
  accessToken: string;
}

const REFRESH_LEEWAY_MS = 30_000;

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private refreshTimer: ReturnType<typeof setTimeout> | undefined;

  private readonly _accessToken = signal<string | null>(null);
  readonly accessToken = this._accessToken.asReadonly();
  readonly isAuthenticated = computed(() => this._accessToken() !== null);

  /** Restores the session on app startup using the HttpOnly refresh-token cookie. */
  async restoreSession(): Promise<void> {
    try {
      await firstValueFrom(this.refreshToken());
    } catch {
      // refreshToken() already cleared the session
    }
  }

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>('/api/auth/login', { email, password }, { withCredentials: true })
      .pipe(tap(res => this.setToken(res.token)));
  }

  logout(): Observable<void> {
    return this.http
      .post<void>('/api/auth/logout', {}, { withCredentials: true })
      .pipe(
        finalize(() => {
          this.setToken(null);
          this.router.navigate(['/login']);
        })
      );
  }

  refreshToken(): Observable<RefreshResponse> {
    return this.http
      .post<RefreshResponse>('/api/auth/refresh', {}, { withCredentials: true })
      .pipe(
        tap(res => this.setToken(res.accessToken)),
        catchError(err => {
          this.setToken(null);
          return throwError(() => err);
        })
      );
  }

  getAccessToken(): string | null {
    return this._accessToken();
  }

  /** Stores the token and schedules a silent refresh shortly before it expires. */
  private setToken(token: string | null): void {
    clearTimeout(this.refreshTimer);
    this._accessToken.set(token);

    const expiresAt = token ? this.getExpiry(token) : null;
    if (expiresAt === null) {
      return;
    }

    const delay = Math.max(expiresAt - Date.now() - REFRESH_LEEWAY_MS, 1_000);
    this.refreshTimer = setTimeout(() => {
      this.refreshToken().subscribe({
        error: () => this.router.navigate(['/login']),
      });
    }, delay);
  }

  /** Returns the JWT exp claim in epoch milliseconds, or null if it cannot be read. */
  private getExpiry(token: string): number | null {
    try {
      const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      const { exp } = JSON.parse(atob(payload)) as { exp?: number };
      return typeof exp === 'number' ? exp * 1000 : null;
    } catch {
      return null;
    }
  }
}