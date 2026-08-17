import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Contact, ContactFilter, ContactSave } from '../models/contact.model';

@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/contacts`;

  getAll(filter?: ContactFilter): Observable<Contact[]> {
    return this.http.get<Contact[]>(this.baseUrl, { params: this.toParams(filter) });
  }

  getById(id: string): Observable<Contact> {
    return this.http.get<Contact>(`${this.baseUrl}/${id}`);
  }

  create(contact: ContactSave): Observable<Contact> {
    return this.http.post<Contact>(this.baseUrl, contact);
  }

  update(id: string, contact: ContactSave): Observable<Contact> {
    return this.http.put<Contact>(`${this.baseUrl}/${id}`, contact);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /** Downloads the filtered contact list as an .xlsx workbook. */
  export(filter?: ContactFilter): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/export`, {
      params: this.toParams(filter),
      responseType: 'blob'
    });
  }

  /**
   * Drops empty members so the query string carries only the filters actually in
   * use. Sending `searchTerm=` would otherwise be a filter the API has to ignore.
   */
  private toParams(filter?: ContactFilter): HttpParams {
    let params = new HttpParams();

    if (!filter) {
      return params;
    }

    for (const [key, value] of Object.entries(filter)) {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    }

    return params;
  }
}
