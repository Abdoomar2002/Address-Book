import { Component, OnInit, computed, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Observable } from 'rxjs';

import { Lookup } from '../../../core/models/lookup.model';
import { LookupService } from '../../../core/services/lookup.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import {
  PromptDialogComponent,
  PromptDialogData
} from '../../../shared/components/prompt-dialog/prompt-dialog.component';

export type LookupKind = 'jobTitle' | 'department';

@Component({
  selector: 'app-lookup-card',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatListModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './lookup-card.component.html',
  styleUrl: './lookup-card.component.css'
})
export class LookupCardComponent implements OnInit {
  private readonly lookupService = inject(LookupService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly title = input.required<string>();
  readonly kind = input.required<LookupKind>();

  readonly items = signal<Lookup[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);

  /** Singular noun used in dialog copy and snackbars. */
  readonly noun = computed(() => (this.kind() === 'jobTitle' ? 'job title' : 'department'));

  // A FormGroup, not a bare FormControl: the form element needs [formGroup] for
  // Angular to own submission, otherwise the browser submits natively and reloads.
  private readonly formBuilder = inject(FormBuilder);

  readonly addForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]]
  });

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.loading.set(true);

    this.listAll().subscribe({
      next: items => {
        this.items.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.items.set([]);
        this.loading.set(false);
      }
    });
  }

  add(): void {
    if (this.addForm.invalid) {
      this.addForm.markAllAsTouched();
      return;
    }

    const name = this.addForm.getRawValue().name.trim();
    this.saving.set(true);

    this.create({ name }).subscribe({
      next: created => {
        this.items.set([...this.items(), created].sort(byName));
        this.addForm.reset({ name: '' });
        this.saving.set(false);
        this.snackBar.open(`${created.name} added.`, 'Dismiss', { duration: 4000 });
      },
      error: () => this.saving.set(false)
    });
  }

  edit(item: Lookup): void {
    this.dialog
      .open<PromptDialogComponent, PromptDialogData, string>(PromptDialogComponent, {
        data: {
          title: `Rename ${this.noun()}`,
          label: 'Name',
          value: item.name
        },
        width: '380px',
        maxWidth: '95vw'
      })
      .afterClosed()
      .subscribe(name => {
        if (!name || name === item.name) {
          return;
        }

        this.update(item.id, { name }).subscribe({
          next: updated => {
            this.items.set(this.items().map(x => (x.id === updated.id ? updated : x)).sort(byName));
            this.snackBar.open(`Renamed to ${updated.name}.`, 'Dismiss', { duration: 4000 });
          }
        });
      });
  }

  remove(item: Lookup): void {
    this.dialog
      .open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
        data: {
          title: `Delete ${this.noun()}`,
          message: `Delete ${item.name}? This cannot be undone.`
        },
        width: '420px',
        maxWidth: '95vw'
      })
      .afterClosed()
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }

        this.delete(item.id).subscribe({
          next: () => {
            this.items.set(this.items().filter(x => x.id !== item.id));
            this.snackBar.open(`${item.name} deleted.`, 'Dismiss', { duration: 4000 });
          }
          // A 409 (still referenced by contacts) is reported by the error interceptor
          // and the row stays put.
        });
      });
  }

  // One card serves both lookups; these pick the matching service calls.

  private listAll(): Observable<Lookup[]> {
    return this.kind() === 'jobTitle'
      ? this.lookupService.getJobTitles()
      : this.lookupService.getDepartments();
  }

  private create(value: { name: string }): Observable<Lookup> {
    return this.kind() === 'jobTitle'
      ? this.lookupService.createJobTitle(value)
      : this.lookupService.createDepartment(value);
  }

  private update(id: string, value: { name: string }): Observable<Lookup> {
    return this.kind() === 'jobTitle'
      ? this.lookupService.updateJobTitle(id, value)
      : this.lookupService.updateDepartment(id, value);
  }

  private delete(id: string): Observable<void> {
    return this.kind() === 'jobTitle'
      ? this.lookupService.deleteJobTitle(id)
      : this.lookupService.deleteDepartment(id);
  }
}

function byName(a: Lookup, b: Lookup): number {
  return a.name.localeCompare(b.name);
}
