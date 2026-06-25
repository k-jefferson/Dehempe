import { Routes } from '@angular/router';
import { Shell } from './layout/shell/shell';

export const routes: Routes = [
  {
    path: '',
    component: Shell,
    children: [
      {
        path: '',
        loadComponent: () => import('./features/home/home').then((m) => m.Home),
        title: 'Accueil — Déhempé',
      },
      // F07 : sélection d'un patient → ses documents (cf. specs/features/F07-...).
      {
        path: 'patient/:ins/documents',
        loadComponent: () => import('./features/documents/documents').then((m) => m.Documents),
        title: 'Documents du patient — Déhempé',
      },
      // F02 : { path: 'patient', loadComponent: ... }
    ],
  },
];
