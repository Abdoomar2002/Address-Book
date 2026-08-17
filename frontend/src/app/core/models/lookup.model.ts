/**
 * Mirrors AddressBook.Application.DTOs.Lookups.
 * Both lookups share the same shape, so Lookup/LookupSave cover either endpoint.
 */

export interface Lookup {
  id: string;
  name: string;
}

export interface LookupSave {
  /** Required, max 100 characters. Must be unique - the API returns 409 on a duplicate. */
  name: string;
}

/** GET/POST/PUT/DELETE /api/jobtitles */
export type JobTitle = Lookup;
export type JobTitleSave = LookupSave;

/** GET/POST/PUT/DELETE /api/departments */
export type Department = Lookup;
export type DepartmentSave = LookupSave;
