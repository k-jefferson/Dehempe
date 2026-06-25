import { BreakpointObserver } from '@angular/cdk/layout';
import { ChangeDetectionStrategy, Component, computed, inject, viewChild } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { filter, map } from 'rxjs';
import { PatientList } from '../../features/patient-list/patient-list';

/** Shell applicatif : toolbar + navigation latérale responsive (cf. specs/design-system/layout-navigation.md). */
@Component({
  selector: 'app-shell',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterOutlet,
    RouterLink,
    MatToolbarModule,
    MatSidenavModule,
    MatIconModule,
    MatButtonModule,
    PatientList,
  ],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  private readonly breakpoints = inject(BreakpointObserver);
  private readonly router = inject(Router);

  /** Tiroir latéral — refermé après navigation en mode mobile (`over`). */
  private readonly sidenav = viewChild(MatSidenav);

  /** Navigation en mode 'over' sous ~960px (tablette / mobile). */
  readonly isHandset = toSignal(
    this.breakpoints.observe('(max-width: 959.98px)').pipe(map((r) => r.matches)),
    { initialValue: false },
  );
  readonly sidenavMode = computed<'over' | 'side'>(() => (this.isHandset() ? 'over' : 'side'));

  constructor() {
    // Sélection d'un patient (F07) → navigation : en mode mobile, refermer le tiroir pour
    // révéler le contenu. Sur desktop (side), le tiroir reste ancré.
    this.router.events
      .pipe(
        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        if (this.isHandset()) {
          this.sidenav()?.close();
        }
      });
  }
}
