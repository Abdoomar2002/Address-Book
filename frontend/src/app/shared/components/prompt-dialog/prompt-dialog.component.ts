import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

export interface PromptDialogData {
  title: string;
  label: string;
  /** Pre-filled value; empty for a new entry. */
  value?: string;
  /** Defaults to 'Save'. */
  confirmLabel?: string;
  maxLength?: number;
}

/**
 * Small single-field prompt. Closes with the trimmed value, or `undefined` on
 * cancel, Escape or backdrop click.
 */
@Component({
  selector: 'app-prompt-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data.title }}</h2>

    <mat-dialog-content>
      <form class="prompt-dialog__form" [formGroup]="form" (ngSubmit)="onSubmit()" id="prompt-form">
        <mat-form-field appearance="outline">
          <mat-label>{{ data.label }}</mat-label>
          <input matInput formControlName="value" cdkFocusInitial />
          @if (form.controls.value.hasError('required')) {
            <mat-error>{{ data.label }} is required.</mat-error>
          } @else if (form.controls.value.hasError('maxlength')) {
            <mat-error>{{ data.label }} must be at most {{ maxLength }} characters.</mat-error>
          }
        </mat-form-field>
      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button type="button" [mat-dialog-close]="undefined">Cancel</button>
      <button mat-flat-button color="primary" type="submit" form="prompt-form">
        {{ data.confirmLabel ?? 'Save' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [
    `
      .prompt-dialog__form {
        display: flex;
        flex-direction: column;
        min-width: min(320px, 70vw);
        padding-top: 8px;
      }
    `
  ]
})
export class PromptDialogComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<PromptDialogComponent, string>);

  readonly data = inject<PromptDialogData>(MAT_DIALOG_DATA);
  readonly maxLength = this.data.maxLength ?? 100;

  readonly form = this.formBuilder.nonNullable.group({
    value: [this.data.value ?? '', [Validators.required, Validators.maxLength(this.maxLength)]]
  });

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dialogRef.close(this.form.getRawValue().value.trim());
  }
}
