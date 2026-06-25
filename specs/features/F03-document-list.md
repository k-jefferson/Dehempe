# F03 — Liste des documents (ITI-18)

> **Statut** : 🔵 En cours · **Priorité** : Haute · **Dépend de** : F05 · **Accès** : sélection patient ([F07](F07-patient-document-navigation.md)) ou saisie INS ([F02](F02-patient-dmp-existence.md)) · **Endpoints** : `GET /api/patients/{ins}/documents`

## 1. Objectif

Pour le **praticien**, **lister les documents du DMP** d'un patient, les filtrer, et en sélectionner un
pour le consulter (F04).

> **Accès & contexte** : l'écran est ouvert via la **route `patient/:ins/documents`**, atteinte en
> **sélectionnant un patient** dans le sidenav ([F07](F07-patient-document-navigation.md)) — ou, à défaut,
> depuis la saisie d'INS ([F02](F02-patient-dmp-existence.md)). L'INS provient du **paramètre de route**
> (`:ins`). L'en-tête rappelle le patient courant (**nom/prénom** issus du jeu d'essai [F06](F06-patient-list.md),
> lookup par INS ; **INS** affiché), sans exiger un passage préalable par F02.

> **État d'implémentation (incrément 1)** : liste fonctionnelle — en-tête contextualisé, fenêtre
> J−30 → aujourd'hui, tableau **triable + paginé** (type / titre / date / auteur, icône dérivée du
> `mimeType`), états chargement / vide (+ panneau diagnostic `dmpRequest`/`dmpResponse`) / 404 / erreur.
> **Reste à faire** : filtre `classCode` multi-valeurs (US-03.2) et ouverture d'un document vers la
> visionneuse F04 (US-03.5).

## 2. Périmètre API

- `GET /api/patients/{ins}/documents?insOid&createdAfter&createdBefore&classCode` → `DocumentList`
  (enveloppe `{ documents, dmpRequest, dmpResponse }`).
- `classCode` est **répétable**. Seuls les documents `APPROVED` sont retournés (pas de filtre de statut).
  Dates ISO 8601 ; défaut Swagger : `createdAfter` = J−30, `createdBefore` = aujourd'hui.
- **Diagnostic** : si `documents` est vide, `dmpRequest`/`dmpResponse` portent le XML SOAP brut (sinon `null`)
  → utile pour l'état vide (afficher la réponse DMP dans un panneau « détails techniques » repliable).
- Auth : **PIN requis** (F05).

## 3. User stories & critères d'acceptation

### US-03.1 — Voir les documents
**En tant que** praticien, **je veux** la liste des documents avec leurs métadonnées clés **afin de** repérer ce qui m'intéresse.
- [x] Colonnes : titre (fallback si `null`), type (`typeCode`/`classCode` lisibles), date (`creationTime`, `dd/MM/yyyy`), auteur (nom lisible dérivé du HL7 `authorPerson`/`authorInstitution`).
- [x] Une icône de type est dérivée du `mimeType` (PDF, etc.).

### US-03.2 — Filtrer
- [ ] Filtre période (`createdAfter` / `createdBefore`), pré-rempli J−30 → aujourd'hui.
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
- Tous les documents retournés sont `APPROVED` (l'API n'interroge pas les `DEPRECATED`).
- **Auteur lisible** : `authorPerson` est un HL7 **XCN** (`id^nom^prénom^…^&autorité&ISO^type`) → afficher
  « Prénom Nom » (repli sur l'identifiant si pas de nom) ; `authorInstitution` (XON) en repli (1re
  composante). La **valeur brute** reste consultable en infobulle (police mono).

## 5. UI / composants (Material 3)

- `mat-table` (tri + pagination), densité locale possible (tableaux denses). Alternative cartes en mobile.
- Filtres via `matChip`/menus ; en-tête de page rappelant le patient/INS courant.

## 6. États & erreurs

- **Chargement** : « Interrogation du DMP… » (skeleton/spinner).
- **Vide** : voir US-03.4. **404** : patient/DMP introuvable.
- **401** : PIN (F05). **502** : erreur DMP.

## 7. Definition of Done

- [ ] US-03.1 → 03.5 · filtres/tri/pagination · états loading/vide/erreur · dates FR · tokens M3 · build OK.
