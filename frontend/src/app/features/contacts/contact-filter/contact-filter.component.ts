import { Component, inject, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { ContactFilter } from '../../../core/models/contact.model';
import { Lookup } from '../../../core/models/lookup.model';
import { LookupService } from '../../../core/services/lookup.service';
import { toIsoDate } from '../../../shared/utils/date.util';

@Component({
  selector: 'app-contact-filter',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './contact-filter.component.html',
  styleUrl: './contact-filter.component.css'
})
export class ContactFilterComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly lookupService = inject(LookupService);

  /** Emitted when the user presses Search or Clear. */
  readonly search = output<ContactFilter>();

  readonly jobTitles = signal<Lookup[]>([]);
  readonly departments = signal<Lookup[]>([]);

  readonly form = this.formBuilder.nonNullable.group({
    searchTerm: [''],
    // '' is the "all" option, so a chosen lookup can be unset again.
    jobTitleId: [''],
    departmentId: [''],
    birthDateFrom: [null as Date | null],
    birthDateTo: [null as Date | null]
  });

  constructor() {
    this.lookupService.getJobTitles().subscribe({ next: jobTitles => this.jobTitles.set(jobTitles) });
    this.lookupService.getDepartments().subscribe({ next: departments => this.departments.set(departments) });
  }

  onSearch(): void {
    this.search.emit(this.buildFilter());
  }

  onClear(): void {
    this.form.reset({
      searchTerm: '',
      jobTitleId: '',
      departmentId: '',
      birthDateFrom: null,
      birthDateTo: null
    });

    // Re-run with an empty filter so the list returns to showing everything.
    this.search.emit({});
  }

  /** Only populated members are included, so the query string stays clean. */
  private buildFilter(): ContactFilter {
    const value = this.form.getRawValue();
    const filter: ContactFilter = {};

    const searchTerm = value.searchTerm.trim();
    if (searchTerm) {
      filter.searchTerm = searchTerm;
    }

    if (value.jobTitleId) {
      filter.jobTitleId = value.jobTitleId;
    }

    if (value.departmentId) {
      filter.departmentId = value.departmentId;
    }

    if (value.birthDateFrom) {
      filter.birthDateFrom = toIsoDate(value.birthDateFrom);
    }

    if (value.birthDateTo) {
      filter.birthDateTo = toIsoDate(value.birthDateTo);
    }

    return filter;
  }
}
