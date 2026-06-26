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
      // F04 : visionneuse de document CDA R2 (N1 PDF ou N3 structuré via XSL ANS).
      {
        path: 'patient/:ins/documents/:uniqueId',
        loadComponent: () => import('./features/document-viewer/document-viewer').then((m) => m.DocumentViewer),
        title: 'Document — Déhempé',
      },
      // F02 : { path: 'patient', loadComponent: ... }
    ],
  },
];
