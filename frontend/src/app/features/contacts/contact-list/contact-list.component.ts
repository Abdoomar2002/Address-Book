import { Component, OnInit, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';

import { Contact } from '../../../core/models/contact.model';
import { ContactService } from '../../../core/services/contact.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ContactFormDialogComponent } from '../contact-form-dialog/contact-form-dialog.component';

/** Well-formed base64: the alphabet plus optional padding. */
const BASE64_PATTERN = /^[A-Za-z0-9+/]+={0,2}$/;

@Component({
  selector: 'app-contact-list',
  standalone: true,
  imports: [MatTableModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatTooltipModule],
  templateUrl: './contact-list.component.html',
  styleUrl: './contact-list.component.css'
})
export class ContactListComponent implements OnInit {
  private readonly contactService = inject(ContactService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly displayedColumns = [
    'photo',
    'fullName',
    'jobTitleName',
    'departmentName',
    'mobileNumber',
    'email',
    'age',
    'actions'
  ];

  readonly contacts = signal<Contact[]>([]);
  readonly loading = signal(true);

  /** Ids whose photo failed to decode, so the row falls back to the placeholder. */
  private readonly brokenPhotos = signal<ReadonlySet<string>>(new Set());

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);

    this.contactService.getAll().subscribe({
      next: contacts => {
        this.contacts.set(contacts);
        this.loading.set(false);
      },
      error: () => {
        // The error interceptor has already shown the API message.
        this.contacts.set([]);
        this.loading.set(false);
      }
    });
  }

  hasPhoto(contact: Contact): boolean {
    return (
      !!contact.photoBase64 &&
      // Cheap charset check: a value that cannot be base64 would make the browser
      // reject the data URL outright (ERR_INVALID_URL) before (error) is useful.
      BASE64_PATTERN.test(contact.photoBase64) &&
      !this.brokenPhotos().has(contact.id)
    );
  }

  photoSrc(contact: Contact): string {
    return `data:image/jpeg;base64,${contact.photoBase64}`;
  }

  /** A stored value that is not decodable image data must not leave a broken icon. */
  onPhotoError(contact: Contact): void {
    this.brokenPhotos.update(ids => new Set(ids).add(contact.id));
  }

  addContact(): void {
    this.openForm(null).subscribe(saved => {
      if (!saved) {
        return;
      }

      // Append locally and re-assign so mat-table sees a new array reference.
      this.contacts.set([...this.contacts(), saved]);
      this.snackBar.open(`${saved.fullName} added.`, 'Dismiss', { duration: 4000 });
    });
  }

  editContact(contact: Contact): void {
    this.openForm(contact).subscribe(saved => {
      if (!saved) {
        return;
      }

      // Replace in place so the row keeps its position in the table.
      this.contacts.set(this.contacts().map(existing => (existing.id === saved.id ? saved : existing)));
      this.brokenPhotos.update(ids => {
        const next = new Set(ids);
        next.delete(saved.id);
        return next;
      });
      this.snackBar.open(`${saved.fullName} updated.`, 'Dismiss', { duration: 4000 });
    });
  }

  deleteContact(contact: Contact): void {
    this.dialog
      .open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
        data: {
          title: 'Delete contact',
          message: `Delete ${contact.fullName}? This cannot be undone.`
        },
        width: '420px',
        maxWidth: '95vw'
      })
      .afterClosed()
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }

        this.contactService.delete(contact.id).subscribe({
          next: () => {
            // Drop locally and re-assign so mat-table sees a new array reference.
            this.contacts.set(this.contacts().filter(existing => existing.id !== contact.id));
            this.snackBar.open(`${contact.fullName} deleted.`, 'Dismiss', { duration: 4000 });
          }
          // The error interceptor has already shown the API message; the row stays put.
        });
      });
  }

  private openForm(contact: Contact | null) {
    return this.dialog
      .open<ContactFormDialogComponent, Contact | null, Contact>(ContactFormDialogComponent, {
        data: contact,
        width: '640px',
        maxWidth: '95vw',
        autoFocus: 'first-tabbable'
      })
      .afterClosed();
  }
}
