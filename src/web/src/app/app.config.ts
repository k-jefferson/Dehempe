import { registerLocaleData } from '@angular/common';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import localeFr from '@angular/common/locales/fr';
import {
  ApplicationConfig,
  LOCALE_ID,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { MAT_ICON_DEFAULT_OPTIONS } from '@angular/material/icon';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { apiKeyInterceptor } from './core/auth/api-key.interceptor';
import { pinInterceptor } from './core/auth/pin.interceptor';
import { frenchPaginatorIntl } from './core/i18n/fr-paginator-intl';

registerLocaleData(localeFr);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes, withComponentInputBinding()),
    // pinInterceptor avant apiKeyInterceptor : le rejeu (next) traverse encore api-key.
    provideHttpClient(withFetch(), withInterceptors([pinInterceptor, apiKeyInterceptor])),
    provideAnimationsAsync(),
    { provide: LOCALE_ID, useValue: 'fr-FR' },
    // Material Symbols Outlined comme jeu d'icônes par défaut (cf. design-system/foundations.md).
    { provide: MAT_ICON_DEFAULT_OPTIONS, useValue: { fontSet: 'material-symbols-outlined' } },
    // Libellés FR du mat-paginator (Material livre l'anglais par défaut).
    { provide: MatPaginatorIntl, useFactory: frenchPaginatorIntl },
  ],
};
