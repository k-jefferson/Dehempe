# F03 — Liste des documents (ITI-18)

> **Statut** : 🟡 Spécifié · **Priorité** : Haute · **Dépend de** : F02, F05 · **Endpoints** : `GET /api/patients/{ins}/documents`

## 1. Objectif

Pour le **praticien**, **lister les documents du DMP** d'un patient, les filtrer, et en sélectionner un
pour le consulter (F04).

## 2. Périmètre API

- `GET /api/patients/{ins}/documents?insOid&createdAfter&createdBefore&status&classCode` → `DocumentEntry[]`.
- `classCode` est **répétable**. `status` défaut `APPROVED`. Dates ISO 8601.
- Auth : **PIN requis** (F05).

## 3. User stories & critères d'acceptation

### US-03.1 — Voir les documents
**En tant que** praticien, **je veux** la liste des documents avec leurs métadonnées clés **afin de** repérer ce qui m'intéresse.
- [ ] Colonnes : titre (fallback si `null`), type (`typeCode`/`classCode` lisibles), date (`creationTime`, `dd/MM/yyyy`), auteur (`authorPerson`/`authorInstitution`), statut.
- [ ] Une icône de type est dérivée du `mimeType` (PDF, etc.).

### US-03.2 — Filtrer
- [ ] Filtre statut `APPROVED` (défaut) / `DEPRECATED`.
- [ ] Filtre période (`createdAfter` / `createdBefore`).
- [ ] Filtre par `classCode` (multi-valeurs).

### US-03.3 — Trier & paginer
- [ ] Tri par date/titre (`matSort`) ; pagination (`mat-paginator`) si nombreux.

### US-03.4 — Aucun document
- [ ] Liste vide → état vide « Ce DMP ne contient aucun document pour les filtres choisis. » + « Réinitialiser les filtres ».

### US-03.5 — Ouvrir un document
- [ ] Sélectionner une ligne → navigue vers la consultation (F04) en transmettant `uniqueId`,
      `repositoryUniqueId` et `homeCommunityId`.

## 4. Règles métier & validations

- Conserver `uniqueId` + `repositoryUniqueId` (+ `homeCommunityId`) de chaque entrée : indispensables à F04.
- Ne pas afficher de date si `null` (afficher « — »).
- `status DEPRECATED` visuellement distinct (badge), mais lisible.

## 5. UI / composants (Material 3)

- `mat-table` (tri + pagination), densité locale possible (tableaux denses). Alternative cartes en mobile.
- Filtres via `matChip`/menus ; en-tête de page rappelant le patient/INS courant.

## 6. États & erreurs

- **Chargement** : « Interrogation du DMP… » (skeleton/spinner).
- **Vide** : voir US-03.4. **404** : patient/DMP introuvable.
- **401** : PIN (F05). **502** : erreur DMP.

## 7. Definition of Done

- [ ] US-03.1 → 03.5 · filtres/tri/pagination · états loading/vide/erreur · dates FR · tokens M3 · build OK.
