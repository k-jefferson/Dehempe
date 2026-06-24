# F05 — Saisie du code PIN (transverse)

> **Statut** : 🟡 Spécifié · **Priorité** : Haute · **Dépend de** : tous les appels DMP (F02–F04)
> **Mécanisme** : en-tête `X-Cps-Pin` (cf. [auth-and-pin-flow](../architecture/auth-and-pin-flow.md))

## 1. Objectif

Permettre au **praticien** de fournir le **code PIN** de sa carte CPS lorsque le DMP l'exige, de façon
**transparente** : le dialog s'ouvre automatiquement, et la requête initiale repart toute seule après saisie.

## 2. Périmètre

- Transverse : déclenché par **toute** réponse `401` avec `errorCode === "CpsPinRequired"`.
- Implémenté par un **intercepteur HTTP fonctionnel** (`pin.interceptor.ts`) + un **dialog** Material
  (`pin-dialog`) + un **store** mémoire (`pin.store.ts`). Voir l'architecture pour le détail du flux.

## 3. User stories & critères d'acceptation

### US-05.1 — On me demande le PIN au bon moment
**En tant que** praticien, **je veux** qu'un dialog me demande mon PIN quand le DMP le réclame.
- [ ] Sur `401 CpsPinRequired`, un dialog masqué s'ouvre (« Saisissez le code PIN de votre carte CPS… »).
- [ ] Une `401` de **clé d'API** (body `{ error }`, pas d'`errorCode`) **n'ouvre pas** ce dialog.

### US-05.2 — Rejeu transparent
- [ ] Après validation, la requête d'origine est **rejouée automatiquement** avec `X-Cps-Pin` ; je vois le résultat sans relancer l'action.

### US-05.3 — Mémorisation de session
- [ ] Le PIN saisi est mémorisé **en mémoire** et réinjecté sur les requêtes suivantes (pas de nouvelle demande tant que la session est valide).
- [ ] Le PIN n'est **jamais** persisté (ni `localStorage`, ni cookie) ni journalisé.

### US-05.4 — PIN incorrect / carte retirée
- [ ] Si une requête **avec** PIN renvoie de nouveau `CpsPinRequired`/erreur d'auth → purge du PIN mémorisé + redemande avec message « Code PIN incorrect ou carte retirée. Réessayez. ».

### US-05.5 — Annulation
- [ ] « Annuler » → la requête échoue proprement (erreur remontée à l'écran appelant), **sans boucle**.

### US-05.6 — Concurrence
- [ ] Plusieurs requêtes en 401 simultanées → **un seul** dialog ; les requêtes en attente réutilisent le PIN saisi.

## 4. Règles métier & validations

- Anti-boucle : **un seul** rejeu automatique par requête.
- Champ PIN : `type="password"`, `inputmode="numeric"`, `autocomplete="off"`, longueur min raisonnable.
- Détection PIN requis : `errorCode === "CpsPinRequired"` (ou header `WWW-Authenticate: CpsPin`).

## 5. UI / composants (Material 3)

- `mat-dialog` : titre « Code PIN », champ masqué, actions « Annuler » / « Valider le code ».
- Focus piégé (CDK), rendu au déclencheur à la fermeture. Message d'erreur en cas de PIN refusé.

## 6. États & erreurs

- **En cours** : indicateur pendant le rejeu.
- **Refusé** : message clair (US-05.4). **Annulé** : pas de blocage.

## 7. Definition of Done

- [ ] US-05.1 → 05.6 · jamais persisté/loggé · anti-boucle · un seul dialog concurrent · a11y dialog · tokens M3 · build OK.
