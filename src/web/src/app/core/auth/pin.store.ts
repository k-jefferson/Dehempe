import { Injectable, computed, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { finalize, map, shareReplay, tap } from 'rxjs/operators';
import { PinDialog, PinDialogData } from './pin-dialog/pin-dialog';

/**
 * État du PIN CPS (cf. specs/architecture/auth-and-pin-flow.md & features/F05).
 * Le PIN est gardé EN MÉMOIRE VIVE uniquement — jamais persisté, jamais journalisé.
 */
@Injectable({ providedIn: 'root' })
export class PinStore {
  private readonly dialog = inject(MatDialog);
  private readonly _pin = signal<string | null>(null);

  readonly pin = this._pin.asReadonly();
  readonly hasPin = computed(() => this._pin() !== null);

  /** Saisie en cours partagée : un seul dialog même si plusieurs requêtes tombent en 401. */
  private pending$: Observable<string | null> | null = null;

  /** Ouvre le dialog (un seul à la fois). Émet le PIN saisi, ou null si annulé. */
  requestPin(message?: string): Observable<string | null> {
    if (this.pending$) {
      return this.pending$;
    }
    const ref = this.dialog.open<PinDialog, PinDialogData, string | null>(PinDialog, {
      data: { message },
      disableClose: true,
      width: '360px',
    });
    this.pending$ = ref.afterClosed().pipe(
      map((pin) => pin ?? null),
      tap((pin) => {
        if (pin) {
          this._pin.set(pin);
        }
      }),
      finalize(() => {
        this.pending$ = null;
      }),
      shareReplay(1),
    );
    return this.pending$;
  }

  /** Purge le PIN mémorisé (PIN refusé / carte retirée). */
  clear(): void {
    this._pin.set(null);
  }
}
