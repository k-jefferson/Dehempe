# Design system — Layout & navigation

## Shell applicatif

Structure unique pour toute l'app (`layout/shell`) :

- **`mat-toolbar`** supérieure (couleur `surface` / `primary` mesuré) :
  - À gauche : bouton menu (mobile) + nom de l'app « Déhempé ».
  - À droite : **identité du praticien** (porteur de la carte CPS) + indicateur d'état carte
    (insérée / PIN fourni). C'est un repère de confiance permanent.
- **`mat-sidenav`** latérale gauche :
  - Desktop (≥ `medium`) : mode `side`, ouverte, ancrée.
  - Mobile (< `medium`) : mode `over`, fermée par défaut, ouverte au bouton menu.
  - En-tête de **marque « Déhempé »** cliquable (retour Accueil, `routerLink="/"`).
  - **Contenu principal = liste des patients** (cf. [F06](../features/F06-patient-list.md)) : un champ
    de recherche épinglé en haut + une liste verticale défilante (un élément par patient, coloré
    selon le sexe). Cette liste **remplace** l'ancien `mat-nav-list` (entrées Accueil / Patient / Documents).
  - *Historique* : tant que F06 n'était pas spécifiée, le sidenav portait un `mat-nav-list` avec item
    actif via `routerLinkActive`. Si un menu de navigation redevient nécessaire (plusieurs écrans
    routés), le réintroduire **en complément** de la liste patients, pas à sa place.
- **Zone de contenu** : `<router-outlet>` dans `mat-sidenav-content`, padding cohérent, largeur de
  lecture **bornée** (`max-width` ~ 1100–1200px, centrée) pour ne pas étirer le texte.

## Responsive

- Utiliser le **CDK `BreakpointObserver`** (pas de media queries dispersées).
- Points de rupture indicatifs : compact < 600, medium 600–1024, expanded ≥ 1024.
- La navigation passe en `over` sous `medium`. Les grilles de cartes passent de multi-colonnes à 1 colonne.
- Cible **desktop d'abord** (poste praticien) mais rester utilisable sur tablette.

## Patron « en-tête de page »

Chaque écran commence par un en-tête homogène :

```
[Titre de page — headline-small]         [actions principales à droite]
[sous-titre / contexte — body-medium on-surface-variant]
```

- Titre = nom de la tâche (« Documents du patient »), pas un terme technique.
- Actions globales de l'écran alignées à droite de l'en-tête.

## Espacement & grille

- Grille **8px** (cf. `foundations.md`). Gouttières de contenu : 16px (compact) → 24px (expanded).
- Espacement vertical entre sections : 24–32px. À l'intérieur d'une carte : 16px.
- Ne pas coller les éléments aux bords : padding de page minimal 16px.

## Densité & lisibilité

- Confort par défaut (density 0). Les **tableaux de documents** peuvent être densifiés localement.
- Largeur de ligne de texte raisonnable ; éviter les blocs pleine largeur sur grand écran.

## Navigation entre écrans

- Profondeur faible. Parcours type : Accueil → Patient (INS) → Documents → Document.
- Conserver l'INS/patient en contexte (en-tête) quand on descend dans les documents.
- Bouton retour explicite quand on entre dans un sous-écran (visionneuse).

## États globaux dans le shell

- Indicateur **carte CPS** (présence + identité) toujours visible dans la toolbar.
- Une 401 PIN ouvre le **dialog PIN** (overlay), pas une navigation (cf. F05).
