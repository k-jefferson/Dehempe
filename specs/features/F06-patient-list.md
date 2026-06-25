# F06 — Liste des patients (sélecteur, données de test)

> **Statut** : 🟢 Implémenté · **Priorité** : Haute · **Dépend de** : — · **Sélection câblée par** : [F07](F07-patient-document-navigation.md) · **Endpoints** : aucun (données locales)

## 1. Objectif

Pour le **praticien**, afficher dans la **navigation latérale** (sidenav) une **liste verticale de
patients** issue d'un **jeu d'essai local**, repérable d'un coup d'œil et **filtrable par nom/prénom**,
afin de choisir rapidement un patient — point d'entrée vers **ses documents** (la sélection est câblée
par [F07](F07-patient-document-navigation.md), qui ouvre la liste de documents F03 ; la saisie manuelle
d'INS F02 reste un chemin alternatif).

Cette liste **remplace** le menu de navigation actuel du sidenav (entrées *Accueil*, *Patient / DMP*,
*Documents*) — voir [§8 Impacts](#8-impacts).

## 2. Périmètre données (pas d'API)

- **Source** : fichier statique **`src/web/src/data/patients-dmp.json`** (jeu d'essai DMP de l'ANS),
  embarqué dans le bundle. **Aucun appel à `Dehempe.API`** ici (ni INS, ni PIN, ni VIHF).
- C'est une **exception assumée** à la règle « les appels HTTP passent par `core/api/*` » : cette
  donnée est purement locale et statique. Elle est exposée via un service **`core/patients`** (et non
  `core/api`), cf. [frontend-architecture](../architecture/frontend-architecture.md).
- **Forme d'un élément** (champs réellement présents dans le JSON) :

  | Champ JSON | Type | Usage UI |
  |---|---|---|
  | `idPatient` | number | clé de liste (`track`) |
  | `nomUtilise` / `nomDeNaissance` | string \| null | **nom affiché** (utilisé, sinon de naissance) |
  | `prenomUtilise` / `prenomDeNaissance` | string \| null | **prénom affiché** (utilisé, sinon de naissance) |
  | `dateDeNaissance` | string ISO `yyyy-MM-dd` | **date de naissance affichée** (`dd/MM/yyyy`) |
  | `sexe` | `"M"` \| `"F"` | **couleur de fond + icône + libellé** (cf. §4) |
  | `matriculeInsNir` | number \| string \| null | **non affiché en ligne** ; présent dans l'infobulle JSON |
  | *(tous les autres champs)* | — | inclus tels quels dans l'infobulle JSON (§US-06.3) |

- **Volumétrie** : ~96 entrées aujourd'hui. `M`/`F` uniquement ; quelques `nomUtilise` à `null`
  (fallback nom de naissance) et un `matriculeInsNir` à `null` (toléré). NIR corses possibles
  (chaîne avec lettre, ex. `188102B17295165`).

## 3. User stories & critères d'acceptation

### US-06.1 — Voir la liste des patients dans le sidenav
**En tant que** praticien, **je veux** une liste verticale des patients dans la barre de gauche **afin de** les parcourir.
- [ ] Le sidenav affiche un élément par patient ; le menu de navigation précédent n'est plus présent.
- [ ] Chaque élément montre **nom**, **prénom** et **date de naissance** (`dd/MM/yyyy`).
- [ ] Nom = `nomUtilise` sinon `nomDeNaissance` ; prénom = `prenomUtilise` sinon `prenomDeNaissance`.
- [ ] La liste défile (scroll) à l'intérieur du sidenav sans déborder la zone de contenu.

### US-06.2 — Distinguer le sexe par la couleur
**En tant que** praticien, **je veux** repérer le sexe d'un coup d'œil.
- [ ] Fond **bleu** si `sexe = "M"`, **rose** si `sexe = "F"`.
- [ ] La couleur n'est **pas le seul indicateur** : l'élément porte aussi une **icône** (`male`/`female`)
      et un **libellé accessible** (« Homme » / « Femme ») — cf. règle a11y, §4.

### US-06.3 — Voir le détail complet en infobulle
**En tant que** praticien, **je veux** survoler un élément pour voir toutes ses données.
- [ ] Une **infobulle** (`matTooltip`) affiche le **JSON complet** du patient (tous les champs,
      indenté et lisible, multi-lignes préservées).

### US-06.4 — Filtrer par nom et prénom
**En tant que** praticien, **je veux** un champ de recherche en haut de la liste **afin de** filtrer.
- [ ] Champ de recherche au-dessus de la liste ; filtrage **en direct** à la frappe.
- [ ] Le filtre s'applique au **nom ET au prénom** (nom utilisé/naissance + prénom utilisé/naissance).
- [ ] Recherche **insensible à la casse et aux accents**, par **sous-chaîne** (« cor » trouve « CORSE »).
- [ ] Bouton d'effacement du champ qui réaffiche la liste complète.

### US-06.5 — Aucun résultat
- [ ] Si aucun patient ne correspond → état vide « Aucun patient ne correspond à votre recherche. »
      + action « Effacer la recherche ».

## 4. Règles métier & validations

- **Nom/prénom affichés** : champ « utilisé » prioritaire, **fallback** sur « de naissance » s'il est `null`.
- **Date de naissance** : `dd/MM/yyyy` via `DatePipe` locale `fr-FR` (cf. [accessibility-i18n](../design-system/accessibility-i18n.md)).
- **Sexe → présentation** :
  | `sexe` | Fond | Icône | Libellé accessible |
  |---|---|---|---|
  | `"M"` | bleu | `male` | « Homme » |
  | `"F"` | rose | `female` | « Femme » |
  | autre / `null` | neutre (`surface-variant`) | `person` | « Sexe non précisé » |
- **a11y — pas de couleur seule** (cf. [accessibility-i18n](../design-system/accessibility-i18n.md)) : le sexe est
  **toujours** doublé d'une icône + d'un texte accessible ; la couleur n'est qu'un renfort.
- **INS non affiché en ligne** (sobriété + lisibilité) ; il reste consultable dans l'infobulle JSON.
- **Ordre d'affichage** : **ordre du fichier** `patients-dmp.json` (aucun tri applicatif).
- **Filtre** : normalisation `toLocaleLowerCase('fr')` + suppression des diacritiques sur la requête et
  sur les champs comparés ; correspondance par inclusion.
- **`track`** par `idPatient`.

## 5. UI / composants (Material 3)

- **Conteneur** : la liste vit dans le `mat-sidenav` du shell (cf. [layout-navigation](../design-system/layout-navigation.md)),
  sous l'en-tête de marque « Déhempé ». Composant dédié `features/patient-list` rendu par le shell.
- **Recherche** : `mat-form-field` `appearance="outline"`, `matInput`, icône `search` en préfixe,
  bouton d'effacement `matIconButton` (`aria-label` « Effacer la recherche ») en suffixe. Épinglée
  en haut (la liste défile dessous).
- **Liste** : `mat-nav-list` (la sélection est câblée par [F07](F07-patient-document-navigation.md) :
  `<a mat-list-item routerLink>` + état actif `routerLinkActive`) ; un `mat-list-item`
  par patient avec : icône de sexe (`matListItemIcon`), titre = `Nom Prénom` (`matListItemTitle`),
  ligne secondaire = date de naissance (`matListItemLine`).
- **Couleur de fond par sexe** (cf. [foundations](../design-system/foundations.md), règle « tokens, pas de hex ») :
  - **Homme (bleu)** → réutiliser le token M3 **`--mat-sys-primary-container`** / `--mat-sys-on-primary-container`
    (bleu issu du seed #1565C0) — aucun token nouveau.
  - **Femme (rose)** → **exception documentée** : aucune palette « rose » n'existe en M3. Définir **une
    seule** paire de tokens applicatifs scoping cette feature, p. ex. dans `src/web/src/styles/` :
    ```scss
    --app-sex-female-container:    light-dark(#FBE3EC, #4B2531);
    --app-sex-female-on-container: light-dark(#3E1F28, #FBE3EC);
    ```
    Lisibles en clair **et** sombre (paire on-/container). Ne pas multiplier les couleurs en dur ailleurs.
  - *(Alternative si l'on préfère la symétrie : définir aussi une paire `--app-sex-male-*` plutôt que
    réutiliser `primary-container`. Voir §9.)*
- **Infobulle JSON** : `matTooltip` = `JSON.stringify(patient, null, 2)` ; `matTooltipClass`
  (`white-space: pre`, police mono, `max-width`/scroll) pour rester lisible. *(Infobulle texte : le
  JSON est de la donnée de test ; acceptable comme confort de mise au point — cf. §9.)*
- **OnPush** + `@for (... ; track p.idPatient)`. Liste longue : `cdk-virtual-scroll` **optionnel**
  (96 items passent sans, à introduire si le jeu grandit).

## 6. États & erreurs

- **Chargement** : si le JSON est chargé via `HttpClient` depuis les assets → bref « Chargement des
  patients… » ; s'il est **importé statiquement** (bundle), aucun état de chargement nécessaire.
- **Aucun résultat de filtre** : voir US-06.5.
- **Données indisponibles** (fichier introuvable / illisible) : message sobre « Liste des patients
  indisponible. » (cas improbable, donnée embarquée).
- *(Pas de 401/PIN/502 : aucune interaction réseau dans cette feature.)*

## 7. Definition of Done

- [ ] US-06.1 → 06.5 satisfaites.
- [ ] Nom/prénom (avec fallback), date `dd/MM/yyyy`.
- [ ] Fond bleu (M) / rose (F) **+ icône + libellé** (jamais la couleur seule).
- [ ] Infobulle JSON complète et lisible.
- [ ] Recherche live nom+prénom, insensible casse/accents, avec effacement ; état « aucun résultat ».
- [ ] Tokens M3 partout, sauf l'unique paire `--app-sex-female-*` documentée ; lisible clair **et** sombre.
- [ ] FR, clavier, `aria-label`, focus visible ; `OnPush` + `track` ; `npm run build` OK.

## 8. Impacts

- **Shell** (`src/web/src/app/layout/shell/`) : le `mat-nav-list` (Accueil / Patient / Documents) est
  remplacé par `<app-patient-list>`. L'accès à *Accueil* est conservé via la **marque « Déhempé »
  rendue cliquable** (`routerLink="/"`). Routes (`app.routes.ts`) inchangées pour l'instant.
- **Spec** [design-system/layout-navigation](../design-system/layout-navigation.md) : description du
  contenu du sidenav mise à jour (liste de patients + recherche au lieu du `mat-nav-list`).
- **Spec** [architecture/frontend-architecture](../architecture/frontend-architecture.md) : arborescence
  (ajout `features/patient-list`, `core/patients`) + mention de la **source de données locale**.
- **F07** [Sélection → documents](F07-patient-document-navigation.md) : câble la sélection — cliquer un
  patient ouvre ses documents (F03) dans la zone de contenu. La liste passe en `mat-nav-list` (item actif).
- **F02** [Existence DMP](F02-patient-dmp-existence.md) : reste un **chemin alternatif** (saisie manuelle
  d'INS) ; il n'est plus sur le chemin de la sélection (qui va directement aux documents via F07).

## 9. Décisions actées (validées avant implémentation)

Décisions validées avec le demandeur :

1. **Sélection → documents (mis à jour)** : ⚠️ La décision initiale « sélection sans action » de la v1
   F06 est **levée**. Le câblage sélection → documents est désormais **spécifié par
   [F07](F07-patient-document-navigation.md)** : cliquer un patient ouvre la liste de ses documents (F03)
   dans la zone de contenu (la liste devient `mat-nav-list` avec item actif). L'affichage / filtrage /
   couleur / infobulle de F06 sont inchangés.
2. **Disparition du menu** : *Patient / DMP* et *Documents* (entrées « Bientôt ») disparaissent du
   sidenav ; *Accueil* passe sur la marque cliquable.
3. **Couleur femme = token applicatif `--app-sex-female-*`** (M3 n'a pas de rose) ; homme = token M3
   `primary-container`.
4. **Infobulle = JSON brut** (donnée de test).
5. **Pas de tri applicatif** : la liste suit l'**ordre du fichier** `patients-dmp.json`.
