# Design system — Fondations Material 3

> Objectif : un design system **Material 3 cohérent et homogène**. Aucune dérive : couleurs,
> typographie, espacements et formes passent **toujours** par les tokens du thème.

## Thème

- **Seed (couleur primaire)** : `#1565C0` — « bleu santé », couleur de confiance, lisible.
- Le thème est défini **une seule fois** dans `src/styles.scss` via le mixin **`mat.theme()`**
  d'Angular Material 21, qui émet les **variables système** M3 (tokens CSS) consommées par tous
  les composants.
- Les palettes M3 (primary / secondary / tertiary / neutral / error) sont **générées depuis le seed**
  dans `src/styles/_theme-colors.scss` (schematic `@angular/material:theme-color`). Ne pas éditer
  les tons à la main : régénérer depuis le seed si la couleur change.

```scss
// src/styles.scss (forme cible)
@use '@angular/material' as mat;
@use './styles/theme-colors' as theme;

html {
  color-scheme: light dark;            // bascule clair/sombre automatique (OS)
  @include mat.theme((
    color: (
      primary: theme.$primary-palette,
      tertiary: theme.$tertiary-palette,
      theme-type: color-scheme,        // light + dark via light-dark()
    ),
    typography: Roboto,
    density: 0,
  ));
}
```

## Rôles de couleur (tokens) — utiliser ceux-ci, pas des hex

| Rôle | Usage |
|---|---|
| `primary` / `on-primary` | Actions principales, éléments porteurs de la marque. |
| `primary-container` / `on-primary-container` | Conteneurs accentués doux (puces, badges actifs). |
| `secondary` / `tertiary` | Accents secondaires, différenciation visuelle mesurée. |
| `surface`, `surface-container*` | Fonds des cartes, barres, feuilles. Hiérarchie par niveaux de container. |
| `on-surface` / `on-surface-variant` | Texte principal / secondaire. |
| `outline` / `outline-variant` | Bordures, séparateurs. |
| `error` / `on-error` / `error-container` | États d'erreur **uniquement**. |

- En SCSS : `color: var(--mat-sys-on-surface);` `background: var(--mat-sys-surface-container);` etc.
- **Interdit** : couleurs en dur (`#1565C0`, `white`, `#333`…) dans les composants. Toujours un token.
- Statut « DMP existant » / succès : ne pas réinventer une palette verte arbitraire — utiliser
  `tertiary`/`primary-container` ou les tokens sémantiques, et un `mat-icon` explicite.

## Mode sombre

- Géré via `color-scheme: light dark` + `theme-type: color-scheme` → bascule automatique selon l'OS.
- Tous les écrans **doivent** rester lisibles en sombre (vérifier le contraste, cf. `accessibility-i18n.md`).

## Typographie

- Police **Roboto** (chargée dans `index.html`). Échelle M3 : `display`, `headline`, `title`, `body`, `label`.
- Classes/utilitaires Material : `mat-headline-small`, `mat-title-medium`, `mat-body-medium`, `mat-label-large`…
- Hiérarchie type :
  - Titre de page → `headline-small` / `title-large`.
  - Titre de carte/section → `title-medium`.
  - Texte courant → `body-medium`.
  - Métadonnées / libellés → `label-large` / `body-small` en `on-surface-variant`.
- Ne pas fixer de `font-size`/`font-weight` en dur : passer par les niveaux typographiques.

## Forme & élévation

- Coins arrondis M3 (tokens `--mat-sys-corner-*`). Cartes : `medium`/`large`. Boutons : `full`/`large`.
- Élévation parcimonieuse : surfaces plates par défaut, élévation pour les éléments flottants (dialog, menu, FAB).

## Espacement & densité

- **Grille de 8px** (4px pour les ajustements fins). Espacements : 4, 8, 12, 16, 24, 32, 40.
- Density M3 = `0` par défaut (confort). Densifier (`-1`/`-2`) **localement** seulement pour des
  zones très denses (tableaux de documents) si nécessaire, jamais globalement sans raison.

## Iconographie

- **Material Symbols** (chargées dans `index.html`), via `<mat-icon>`.
- Style cohérent : un seul jeu (outlined recommandé), même graisse. Toujours doubler l'icône d'un
  libellé ou d'un `aria-label` (cf. accessibilité).

## Definition of Done « design »

- [ ] Zéro couleur/typo/rayon en dur : que des tokens M3.
- [ ] Lisible en clair **et** sombre.
- [ ] Espacements sur la grille 8px.
- [ ] Composants Material standard réutilisés (pas de réimplémentation maison d'un bouton/carte).
