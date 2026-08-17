import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

const TOKEN_KEY = 'ab.token';
const EXPIRES_AT_KEY = 'ab.expiresAt';
const FULL_NAME_KEY = 'ab.fullName';
const EMAIL_KEY = 'ab.email';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/login`, request)
      .pipe(tap(response => this.storeSession(response)));
  }

  /**
   * Creates the account but deliberately does NOT start a session: the flow sends the
   * user to the login page to sign in themselves. The API returns a token here, and
   * storing it would leave them logged in while sitting on /login.
   */
  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/register`, request);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EXPIRES_AT_KEY);
    localStorage.removeItem(FULL_NAME_KEY);
    localStorage.removeItem(EMAIL_KEY);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  /**
   * True only while a stored token is still within its lifetime. Checking the
   * expiry locally avoids firing requests that are already guaranteed to 401.
   */
  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    const expiresAt = localStorage.getItem(EXPIRES_AT_KEY);
    if (!expiresAt) {
      return false;
    }

    // The API sends a UTC instant with a trailing Z, so Date.parse is unambiguous.
    const expiry = Date.parse(expiresAt);
    if (Number.isNaN(expiry)) {
      return false;
    }

    return expiry > Date.now();
  }

  getFullName(): string | null {
    return localStorage.getItem(FULL_NAME_KEY);
  }

  getEmail(): string | null {
    return localStorage.getItem(EMAIL_KEY);
  }

  private storeSession(response: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(EXPIRES_AT_KEY, response.expiresAt);
    localStorage.setItem(FULL_NAME_KEY, response.fullName);
    localStorage.setItem(EMAIL_KEY, response.email);
  }
}
