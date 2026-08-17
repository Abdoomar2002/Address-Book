/**
 * Mirrors AddressBook.Application.DTOs.Contacts.
 * Guid maps to string; DateTime arrives as an ISO-8601 string over JSON.
 */

/** Read model returned by GET /api/contacts. Never carries a password. */
export interface Contact {
  id: string;
  fullName: string;
  email: string;
  mobileNumber: string;
  /** ISO-8601, e.g. '1990-05-20T00:00:00'. */
  dateOfBirth: string;
  /** Computed server-side from dateOfBirth; read-only. */
  age: number;
  address: string;
  jobTitleId: string;
  jobTitleName: string;
  departmentId: string;
  departmentName: string;
  photoBase64: string;
}

/** Write model for POST and PUT /api/contacts. */
export interface ContactSave {
  fullName: string;
  jobTitleId: string;
  departmentId: string;
  /** Egyptian mobile, e.g. '01012345678'. */
  mobileNumber: string;
  /** Must be a past date. */
  dateOfBirth: string;
  address: string;
  email: string;
  /** Required when creating. Omit on update to leave the existing password unchanged. */
  password?: string;
  photoBase64: string;
}

/** Query parameters for GET /api/contacts and /api/contacts/export. All optional. */
export interface ContactFilter {
  searchTerm?: string;
  jobTitleId?: string;
  departmentId?: string;
  /** Inclusive lower bound on date of birth. */
  birthDateFrom?: string;
  /** Inclusive upper bound on date of birth. */
  birthDateTo?: string;
}
