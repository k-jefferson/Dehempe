import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

export interface PinDialogData {
  message?: string;
}

/** Dialog de saisie du code PIN CPS (cf. specs/features/F05-pin-entry.md). */
@Component({
  selector: 'app-pin-dialog',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './pin-dialog.html',
  styleUrl: './pin-dialog.scss',
})
export class PinDialog {
  private readonly ref = inject(MatDialogRef<PinDialog, string | null>);
  private readonly data = inject<PinDialogData>(MAT_DIALOG_DATA, { optional: true });

  readonly message =
    this.data?.message ?? 'Saisissez le code PIN de votre carte CPS pour accéder au DMP.';
  readonly pin = signal('');

  validate(): void {
    const value = this.pin().trim();
    if (value) {
      this.ref.close(value);
    }
  }

  cancel(): void {
    this.ref.close(null);
  }
}
