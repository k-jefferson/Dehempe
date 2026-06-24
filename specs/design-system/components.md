# Design system — Catalogue de composants

Quel composant Material 3 utiliser pour quel usage. **Réutiliser les composants Angular Material** ;
ne pas réimplémenter un bouton/carte/dialog maison.

## Actions — boutons

| Besoin | Composant |
|---|---|
| Action principale d'un écran | `matButton="filled"` (1 seule par zone). |
| Action secondaire | `matButton="outlined"` ou `matButton` (texte). |
| Action tonale (mise en avant douce) | `matButton="tonal"`. |
| Action sur icône seule | `matIconButton` + `aria-label` obligatoire. |
| Action flottante (création) | `matFab` / `matMiniFab` (rare en v1, lecture seule). |

- Hiérarchie : **une** action filled par zone, le reste outlined/texte.
- Libellés verbe + complément (« Consulter le document »), cf. `ux-writing.md`.

## Saisie — formulaires

- `mat-form-field` **appearance `outline`** (cohérence globale), avec `mat-label`.
- Champ INS : `matInput`, `inputmode="numeric"`, masque/format lisible (cf. `accessibility-i18n.md`),
  `mat-error` pour la validation, `mat-hint` pour aider (« NIR : 15 chiffres »).
- PIN : `matInput type="password"`, `inputmode="numeric"`, `autocomplete="off"` (cf. F05).
- Réactif : `ReactiveFormsModule` + validation FluentValidation-compatible côté affichage.

## Conteneurs — surfaces

| Besoin | Composant |
|---|---|
| Regrouper des infos (identité praticien, fiche patient) | `mat-card` (variant `outlined` par défaut, sobre). |
| Liste d'éléments cliquables | `mat-list` / `mat-nav-list` ou cartes en grille. |
| Beaucoup de documents tabulaires | `mat-table` (tri `matSort`, pagination `mat-paginator`). |
| Sections repliables (détails techniques SOAP) | `mat-expansion-panel` (replié par défaut). |
| Regrouper par onglets | `mat-tabs` (avec parcimonie). |

## Navigation

- `mat-toolbar` (barre supérieure) + `mat-sidenav` (navigation latérale) — voir `layout-navigation.md`.
- `mat-nav-list` pour les entrées de menu, item actif via `routerLinkActive`.
- Fil d'Ariane simple si profondeur > 2.

## Retour d'information (états)

| État | Composant / pattern |
|---|---|
| Chargement court | `mat-progress-bar` (indéterminée) en tête de zone, ou `mat-spinner`. |
| Chargement de liste | squelettes ou spinner centré + message « Interrogation du DMP… ». |
| Message transitoire | `mat-snack-bar` (succès/erreur non bloquante). |
| Infobulle au survol (aide, détail) | `matTooltip` (texte ; `matTooltipClass` pour un contenu pré-formaté, ex. JSON de test en F06). |
| Confirmation / saisie modale | `mat-dialog` (ex. dialog PIN F05). |
| **Vide** (0 document, pas de DMP) | bloc « empty state » : icône + titre + courte explication + action. |
| **Erreur** | bloc dédié (icône `error` token `error`), message clair, bouton « Réessayer ». |
| Badge / statut | `matChip` / `matBadge` (ex. statut `APPROVED`/`DEPRECATED`). |

## Données patient / document

- Identité (praticien, patient) → `mat-card` avec lignes label/valeur (`label-large` / `body-medium`).
- Dates → toujours formatées FR (`dd/MM/yyyy`), cf. `accessibility-i18n.md`.
- INS / RPPS → formatage lisible, jamais tronqué silencieusement.
- Contenu document (PDF…) → ouverture via blob (`F04`), visionneuse ou téléchargement selon `mimeType`.

## Règles transverses

- **OnPush** partout ; listes avec `track`.
- Tout `mat-icon-button` / icône informative → `aria-label`.
- Pas de composant maison si un composant Material couvre le besoin.
- États **loading / vide / erreur** prévus pour **chaque** écran qui appelle l'API (jamais d'écran « blanc » silencieux).
