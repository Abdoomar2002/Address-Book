import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';

import { Contact, ContactSave } from '../../../core/models/contact.model';
import { Lookup } from '../../../core/models/lookup.model';
import { ContactService } from '../../../core/services/contact.service';
import { LookupService } from '../../../core/services/lookup.service';

/** Egyptian mobile, matching the RegularExpression on ContactSaveDto. */
const EGYPT_MOBILE_PATTERN = /^(\+20|0020|0)?1[0125]\d{8}$/;

/** Roughly 1 MB of image, which is about 1.37 MB once base64-encoded. */
const MAX_PHOTO_BYTES = 1_000_000;

@Component({
  selector: 'app-contact-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './contact-form-dialog.component.html',
  styleUrl: './contact-form-dialog.component.css'
})
export class ContactFormDialogComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly contactService = inject(ContactService);
  private readonly lookupService = inject(LookupService);
  private readonly dialogRef = inject(MatDialogRef<ContactFormDialogComponent, Contact>);

  /** null means "add"; a contact means "edit". */
  readonly contact = inject<Contact | null>(MAT_DIALOG_DATA);

  readonly isEdit = this.contact !== null;
  readonly title = this.isEdit ? 'Edit contact' : 'Add contact';
  readonly submitLabel = this.isEdit ? 'Save changes' : 'Add contact';

  /** The datepicker must not offer a future date of birth. */
  readonly today = new Date();

  readonly jobTitles = signal<Lookup[]>([]);
  readonly departments = signal<Lookup[]>([]);
  readonly loadingLookups = signal(true);
  readonly saving = signal(false);
  readonly photoError = signal<string | null>(null);

  /** Raw base64, no data: prefix - that is what the API stores. */
  readonly photoBase64 = signal(this.contact?.photoBase64 ?? '');
  readonly photoPreview = computed(() =>
    this.photoBase64() ? `data:image/jpeg;base64,${this.photoBase64()}` : null
  );

  readonly form = this.formBuilder.nonNullable.group({
    fullName: [this.contact?.fullName ?? '', [Validators.required, Validators.maxLength(200)]],
    jobTitleId: [this.contact?.jobTitleId ?? '', [Validators.required]],
    departmentId: [this.contact?.departmentId ?? '', [Validators.required]],
    mobileNumber: [
      this.contact?.mobileNumber ?? '',
      [Validators.required, Validators.pattern(EGYPT_MOBILE_PATTERN)]
    ],
    dateOfBirth: [this.contact ? new Date(this.contact.dateOfBirth) : null as Date | null, [Validators.required]],
    address: [this.contact?.address ?? '', [Validators.required, Validators.maxLength(255)]],
    email: [
      this.contact?.email ?? '',
      [Validators.required, Validators.email, Validators.maxLength(255)]
    ],
    // Adding requires a password; editing leaves it blank to keep the current one.
    password: ['', this.isEdit ? [Validators.minLength(6)] : [Validators.required, Validators.minLength(6)]]
  });

  constructor() {
    this.loadLookups();
  }

  private loadLookups(): void {
    this.loadingLookups.set(true);

    this.lookupService.getJobTitles().subscribe({
      next: jobTitles => {
        this.jobTitles.set(jobTitles);
        this.loadingLookups.set(false);
      },
      error: () => this.loadingLookups.set(false)
    });

    this.lookupService.getDepartments().subscribe({
      next: departments => this.departments.set(departments)
    });
  }

  onPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    if (!file.type.startsWith('image/')) {
      this.photoError.set('Choose an image file.');
      return;
    }

    if (file.size > MAX_PHOTO_BYTES) {
      this.photoError.set('Image must be smaller than 1 MB.');
      return;
    }

    this.photoError.set(null);

    const reader = new FileReader();
    reader.onload = () => {
      const dataUrl = String(reader.result);
      // Strip "data:<mime>;base64," - the API stores the payload only.
      this.photoBase64.set(dataUrl.slice(dataUrl.indexOf(',') + 1));
    };
    reader.onerror = () => this.photoError.set('Could not read that file.');
    reader.readAsDataURL(file);
  }

  clearPhoto(): void {
    this.photoBase64.set('');
    this.photoError.set(null);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (!this.photoBase64()) {
      this.photoError.set('A photo is required.');
      return;
    }

    this.saving.set(true);
    this.form.disable();

    const payload = this.buildPayload();

    const save$ = this.isEdit
      ? this.contactService.update(this.contact!.id, payload)
      : this.contactService.create(payload);

    save$.subscribe({
      next: saved => this.dialogRef.close(saved),
      error: () => {
        // The error interceptor has already shown the API message.
        this.saving.set(false);
        this.form.enable();
      }
    });
  }

  private buildPayload(): ContactSave {
    const value = this.form.getRawValue();
    const dateOfBirth = value.dateOfBirth as Date;

    const payload: ContactSave = {
      fullName: value.fullName.trim(),
      jobTitleId: value.jobTitleId,
      departmentId: value.departmentId,
      mobileNumber: value.mobileNumber.trim(),
      // Send a plain calendar date so a timezone offset cannot shift the day.
      dateOfBirth: toIsoDate(dateOfBirth),
      address: value.address.trim(),
      email: value.email.trim(),
      photoBase64: this.photoBase64()
    };

    // Omitted on edit when left blank, which tells the API to keep the current password.
    if (value.password) {
      payload.password = value.password;
    }

    return payload;
  }
}

/** Local calendar date as yyyy-MM-dd, avoiding the UTC shift of toISOString(). */
function toIsoDate(date: Date): string {
  const month = `${date.getMonth() + 1}`.padStart(2, '0');
  const day = `${date.getDate()}`.padStart(2, '0');

  return `${date.getFullYear()}-${month}-${day}`;
}
