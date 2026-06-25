# Architecture frontend

## Stack

- **Angular 21** (standalone, sans NgModule), **TypeScript strict**.
- **Angular Material 21 + CDK** — design system **Material 3** (voir `design-system/`).
- **SCSS** pour les styles ; thème M3 centralisé dans `src/styles.scss` (+ `src/styles/_theme-colors.scss`).
- **RxJS** pour l'HTTP ; **Signals** pour l'état local des composants/services.
- Build & serve via Angular CLI (`@angular/build`). Proxy de dev pour l'API.

## Conventions Angular 21 (importantes)

- **Composants standalone** uniquement. Pas de `NgModule`.
- **Nommage CLI nouveau** : `xxx.ts` / `xxx.html` / `xxx.scss` (pas de suffixe `.component`).
  Le composant racine est `app.ts` (classe `App`), la conf `app.config.ts`, les routes `app.routes.ts`.
- **`ChangeDetectionStrategy.OnPush`** par défaut sur tous les composants.
- **Signals** pour l'état (`signal`, `computed`, `effect`) ; `input()` / `output()` fonctionnels ;
  `inject()` plutôt que l'injection par constructeur.
- **Control flow natif** dans les templates : `@if`, `@for`, `@switch` (pas `*ngIf`/`*ngFor`).
- **HttpClient fonctionnel** : `provideHttpClient(withInterceptors([...]))`, intercepteurs **fonctionnels**
  (`HttpInterceptorFn`), pas de classe.
- **Lazy loading** des features via `loadComponent` / `loadChildren` dans les routes.

## Arborescence cible

```
src/web/src/
├── main.ts
├── index.html                  ← lang="fr", polices Roboto + Material Symbols, <app-root>
├── styles.scss                 ← thème M3 global (mat.theme) + resets + utilitaires
├── styles/
│   └── _theme-colors.scss      ← palettes M3 générées depuis le seed #1565C0
├── data/
│   └── patients-dmp.json       ← jeu d'essai local des patients (source de F06, hors API)
├── environments/
│   ├── environment.ts          ← prod : apiBaseUrl, apiKey
│   └── environment.development.ts
└── app/
    ├── app.ts / app.html / app.scss / app.config.ts / app.routes.ts
    ├── core/                   ← singletons, sans UI réutilisable de feature
    │   ├── api/
    │   │   ├── models/         ← interfaces calquées sur les DTOs (cf. api-contract.md)
    │   │   ├── cps-api.ts            (CpsApi)        GET /api/cps/card
    │   │   ├── patients-api.ts       (PatientsApi)   GET /api/patients/{ins}/dmp
    │   │   └── documents-api.ts      (DocumentsApi)  liste + contenu
    │   ├── auth/
    │   │   ├── pin.store.ts          ← état PIN (signal), mémoire vive uniquement
    │   │   ├── pin.interceptor.ts    ← 401 CpsPinRequired → dialog → rejeu X-Cps-Pin
    │   │   ├── api-key.interceptor.ts← ajoute X-Api-Key si environment.apiKey
    │   │   └── pin-dialog/           ← composant dialog M3 (F05)
    │   ├── patients/           ← données patients LOCALES (jeu d'essai, hors API)
    │   │   ├── test-patient.ts       ← interface calquée sur data/patients-dmp.json
    │   │   └── patients-dataset.ts   ← (PatientsDataset) charge le JSON, expose un signal (F06)
    │   └── http/
    │       └── api-base.interceptor.ts (optionnel : préfixe apiBaseUrl)
    ├── layout/
    │   └── shell/              ← toolbar + sidenav responsive ; le sidenav héberge la liste patients (F06)
    ├── features/               ← une feature = un dossier (F01…F07 à venir)
    │   ├── home/               ← page d'accueil (placeholder au démarrage)
    │   ├── patient-list/       ← liste de patients filtrable + sélection→documents (F06/F07), sidenav
    │   └── documents/          ← liste des documents d'un patient (F03/F04, à venir)
    └── shared/                 ← composants/pipes/directives réutilisables transverses
```

## Règles de dépendances

- `features/*` peut dépendre de `core/*`, `shared/*`, `@angular/material`.
- `core/*` ne dépend **pas** de `features/*`.
- `shared/*` = présentationnel et générique, **sans** logique métier ni appel API.
- Les **appels HTTP** vers `Dehempe.API` se font **uniquement** via les services `core/api/*` (jamais `HttpClient` direct dans une feature).
- **Donnée locale statique** (jeu d'essai patients, F06) : exposée par `core/patients/*` (et **non**
  `core/api/*`, car ce n'est pas un endpoint backend). Le JSON est embarqué (`src/data/patients-dmp.json`) ;
  selon l'implémentation il est **importé** (`resolveJsonModule`) ou lu via `HttpClient` depuis les assets
  — à déclarer dans `angular.json` si servi en asset. Aucune donnée patient locale n'est persistée sur disque.

## État & données

- État **local** d'écran → Signals dans le composant.
- État **partagé** (ex. praticien courant, PIN) → service `core` exposant des Signals (`*.store.ts`).
- Pas de store global lourd (NgRx) en v1 ; rester sur Signals + services tant que ça suffit.
- **Aucune persistance disque** de données patient / PIN (cf. `auth-and-pin-flow.md`).

## Routing

- Routes **lazy** ; le `shell` est le layout parent, les features sont ses enfants.
- Esquisse (à compléter quand les features arrivent) :
  ```
  '' → shell
       ├── ''            → home (accueil / état carte CPS)
       ├── 'patient'     → recherche + existence DMP (F02)        [lazy]
       └── 'patient/:ins/documents' → liste + visionneuse (F03/F04) [lazy]  ← cible de la sélection F07
  ```
- **Sélection patient (F07)** : les items de `patient-list` sont des liens
  `routerLink="/patient/{{ins}}/documents"` (`mat-nav-list`, item actif via `routerLinkActive`).
  L'INS vient de `matriculeInsNir` (stringifié) ; les patients sans INS ne sont pas navigables.

## Qualité

- `npm run build` doit passer **sans erreur ni warning bloquant** avant tout commit.
- `OnPush` + `trackBy`/`track` sur les listes.
- Pas de valeurs magiques de style : passer par les tokens M3 (cf. `design-system/foundations.md`).
- i18n : libellés en français (cf. `design-system/accessibility-i18n.md`).

## Build & exécution (rappel)

```bash
npm --prefix src/web start     # ng serve + proxy → http://localhost:4200
npm --prefix src/web run build # build de prod
```
> Le proxy `proxy.conf.json` redirige `/api` vers `https://localhost:7270` : démarrer aussi
> `Dehempe.API` (profil https) pour les appels réels.
