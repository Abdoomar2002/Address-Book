import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

export interface ConfirmDialogData {
  title: string;
  message: string;
  /** Defaults to 'Yes'. */
  confirmLabel?: string;
  /** Defaults to 'Cancel'. */
  cancelLabel?: string;
  /** Material colour for the confirm button. Defaults to 'warn'. */
  confirmColor?: 'primary' | 'accent' | 'warn';
}

/**
 * Reusable yes/no confirmation. Closes with `true` on confirm and `undefined`
 * on cancel, backdrop click or Escape - so callers only act on an explicit yes.
 */
@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data.title }}</h2>

    <mat-dialog-content>
      <p class="confirm-dialog__message">{{ data.message }}</p>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button [mat-dialog-close]="undefined">{{ data.cancelLabel ?? 'Cancel' }}</button>
      <button mat-flat-button [color]="data.confirmColor ?? 'warn'" [mat-dialog-close]="true" cdkFocusInitial>
        {{ data.confirmLabel ?? 'Yes' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [
    `
      .confirm-dialog__message {
        margin: 0;
        max-width: 46ch;
      }
    `
  ]
})
export class ConfirmDialogComponent {
  readonly data = inject<ConfirmDialogData>(MAT_DIALOG_DATA);
}
