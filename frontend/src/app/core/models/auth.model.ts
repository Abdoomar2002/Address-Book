/** Mirrors AddressBook.Application.DTOs.Auth. */

/** POST /api/auth/register */
export interface RegisterRequest {
  fullName: string;
  email: string;
  /** Minimum 6 characters. */
  password: string;
  /** Must equal password. */
  confirmPassword: string;
}

/** POST /api/auth/login */
export interface LoginRequest {
  email: string;
  password: string;
}

/** Returned by both register and login. */
export interface AuthResponse {
  /** JWT to send as `Authorization: Bearer <token>`. */
  token: string;
  fullName: string;
  email: string;
  /** ISO-8601 UTC instant at which the token stops validating. */
  expiresAt: string;
}
