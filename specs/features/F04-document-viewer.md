# F04 — Consultation d'un document (ITI-43)

> **Statut** : 🟡 Spécifié · **Priorité** : Moyenne · **Dépend de** : F03, F05 · **Endpoints** : `GET …/documents/{uniqueId}/content`

## 1. Objectif

Pour le **praticien**, **consulter et/ou télécharger le contenu** d'un document du DMP sélectionné dans la liste (F03).

## 2. Périmètre API

- `GET /api/patients/{ins}/documents/{uniqueId}/content?repositoryUniqueId&homeCommunityId` → **binaire**.
- `uniqueId` **URL-encodé** ; `repositoryUniqueId` obligatoire ; `Content-Type` = `mimeType`.
- Récupération en `responseType: 'blob'`. Auth : **PIN requis** (F05).

## 3. User stories & critères d'acceptation

### US-04.1 — Consulter le document
**En tant que** praticien, **je veux** afficher le contenu (PDF en priorité) **afin de** le lire sans quitter l'app.
- [ ] PDF → visionneuse intégrée (object URL du blob).
- [ ] Le type est respecté via le `Content-Type` renvoyé.

### US-04.2 — Télécharger
- [ ] Bouton « Télécharger » → enregistre le fichier avec un nom lisible (titre + extension dérivée du `mimeType`).

### US-04.3 — Type non affichable
- [ ] Si le type n'est pas affichable dans le navigateur → proposer directement le téléchargement avec un message clair.

### US-04.4 — Erreurs
- [ ] `404` → « Document introuvable. » `401` → PIN (F05).

## 4. Règles métier & validations

- Toujours partir des identifiants fournis par F03 (`uniqueId`, `repositoryUniqueId`, `homeCommunityId`).
- Encoder `uniqueId` (`encodeURIComponent`).
- **Révoquer** les object URLs créés (`URL.revokeObjectURL`) à la fermeture (pas de fuite mémoire).
- Le contenu n'est **pas** persisté sur disque par l'app (téléchargement = action explicite utilisateur).

## 5. UI / composants (Material 3)

- En-tête avec titre du document + bouton retour (vers F03) + bouton « Télécharger ».
- Zone visionneuse (PDF) ; sinon carte « Aperçu indisponible » + téléchargement.

## 6. États & erreurs

- **Chargement** : « Récupération du document… ».
- **Erreur** : 404 / 401 / 502 (cf. ux-writing).
- **Non supporté** : aperçu indisponible → téléchargement.

## 7. Definition of Done

- [ ] US-04.1 → 04.4 · object URL révoqué · états loading/erreur · tokens M3 · build OK.
