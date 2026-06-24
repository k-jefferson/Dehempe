import { BreakpointObserver } from '@angular/cdk/layout';
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { map } from 'rxjs';

interface NavItem {
  label: string;
  icon: string;
  /** Route absolue. Absent = entrée « à venir » (désactivée). */
  path?: string;
  hint?: string;
}

/** Shell applicatif : toolbar + navigation latérale responsive (cf. specs/design-system/layout-navigation.md). */
@Component({
  selector: 'app-shell',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
  ],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  private readonly breakpoints = inject(BreakpointObserver);

  /** Navigation en mode 'over' sous ~960px (tablette / mobile). */
  readonly isHandset = toSignal(
    this.breakpoints.observe('(max-width: 959.98px)').pipe(map((r) => r.matches)),
    { initialValue: false },
  );
  readonly sidenavMode = computed<'over' | 'side'>(() => (this.isHandset() ? 'over' : 'side'));

  readonly nav: readonly NavItem[] = [
    { label: 'Accueil', icon: 'home', path: '/' },
    { label: 'Patient / DMP', icon: 'badge', hint: 'Bientôt' },
    { label: 'Documents', icon: 'folder_shared', hint: 'Bientôt' },
  ];
}
