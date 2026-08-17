import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Department, DepartmentSave, JobTitle, JobTitleSave, Lookup, LookupSave } from '../models/lookup.model';

@Injectable({ providedIn: 'root' })
export class LookupService {
  private readonly http = inject(HttpClient);
  private readonly jobTitlesUrl = `${environment.apiUrl}/jobtitles`;
  private readonly departmentsUrl = `${environment.apiUrl}/departments`;

  // Job titles

  getJobTitles(): Observable<JobTitle[]> {
    return this.getAll(this.jobTitlesUrl);
  }

  getJobTitle(id: string): Observable<JobTitle> {
    return this.getById(this.jobTitlesUrl, id);
  }

  createJobTitle(jobTitle: JobTitleSave): Observable<JobTitle> {
    return this.create(this.jobTitlesUrl, jobTitle);
  }

  updateJobTitle(id: string, jobTitle: JobTitleSave): Observable<JobTitle> {
    return this.update(this.jobTitlesUrl, id, jobTitle);
  }

  /** Fails with 409 if any contact still references the job title. */
  deleteJobTitle(id: string): Observable<void> {
    return this.delete(this.jobTitlesUrl, id);
  }

  // Departments

  getDepartments(): Observable<Department[]> {
    return this.getAll(this.departmentsUrl);
  }

  getDepartment(id: string): Observable<Department> {
    return this.getById(this.departmentsUrl, id);
  }

  createDepartment(department: DepartmentSave): Observable<Department> {
    return this.create(this.departmentsUrl, department);
  }

  updateDepartment(id: string, department: DepartmentSave): Observable<Department> {
    return this.update(this.departmentsUrl, id, department);
  }

  /** Fails with 409 if any contact still references the department. */
  deleteDepartment(id: string): Observable<void> {
    return this.delete(this.departmentsUrl, id);
  }

  // Both lookups expose the same five endpoints, so the calls live in one place.

  private getAll(url: string): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(url);
  }

  private getById(url: string, id: string): Observable<Lookup> {
    return this.http.get<Lookup>(`${url}/${id}`);
  }

  private create(url: string, lookup: LookupSave): Observable<Lookup> {
    return this.http.post<Lookup>(url, lookup);
  }

  private update(url: string, id: string, lookup: LookupSave): Observable<Lookup> {
    return this.http.put<Lookup>(`${url}/${id}`, lookup);
  }

  private delete(url: string, id: string): Observable<void> {
    return this.http.delete<void>(`${url}/${id}`);
  }
}
