# F02 — Test d'existence du DMP (TD 0.2)

> **Statut** : 🟡 Spécifié · **Priorité** : Haute · **Dépend de** : F05 (PIN) · **Endpoints** : `GET /api/patients/{ins}/dmp`

## 1. Objectif

Pour le **praticien**, savoir **si un patient possède un DMP** à partir de son INS, et voir les
informations d'identité retournées, afin de décider s'il peut consulter ses documents.

## 2. Périmètre API

- `GET /api/patients/{ins}/dmp?insOid={oid}` → `DmpExistence` (cf. [api-contract](../architecture/api-contract.md)).
- Auth : **PIN requis** → la 1ʳᵉ requête peut déclencher le dialog PIN (F05).

## 3. User stories & critères d'acceptation

### US-02.1 — Saisir un INS et lancer le test
**En tant que** praticien, **je veux** saisir l'INS d'un patient et lancer la recherche **afin de** interroger le DMP.
- [ ] Champ « INS du patient » (`inputmode="numeric"`), bouton « Rechercher le DMP ».
- [ ] `insOid` déduit automatiquement : 15 chiffres → NIR (`1.2.250.1.213.1.4.8`), sinon NIA (`1.2.250.1.213.1.4.9`).

### US-02.2 — Connaître l'existence du DMP
**En tant que** praticien, **je veux** un résultat clair existant / inexistant.
- [ ] `exists === true` → indicateur « DMP trouvé » ; `false` → état vide « Aucun DMP pour ce patient ».

### US-02.3 — Voir l'identité patient
**En tant que** praticien, **je veux** voir les infos patient renvoyées (`patient`) quand le DMP existe.
- [ ] Affiche nom/prénom, date de naissance (`dd/MM/yyyy`), sexe (libellé FR), INS, et indicateurs
      `isAttachedToTreatingPhysician` / `isAuthorizationValid` sous forme lisible.

### US-02.4 — INS invalide
- [ ] `400` → message de validation près du champ (« INS invalide. NIR : 15 chiffres. »).

### US-02.5 — Première interrogation (PIN)
- [ ] `401 CpsPinRequired` → dialog PIN (F05), puis affichage du résultat sans nouvelle action manuelle.

## 4. Règles métier & validations

- Validation INS : chiffres uniquement ; longueur cohérente (15 pour NIR).
- `errorMessage` du DTO, s'il est présent, est affiché de façon lisible.
- Le champ `request` (XML SOAP) n'est **jamais** affiché par défaut (bloc debug replié).

## 5. UI / composants (Material 3)

- En-tête de page + `mat-form-field` (outline) + bouton filled.
- Résultat dans une `mat-card` ; statut via icône + texte (pas couleur seule).
- Détails techniques repliés (`mat-expansion-panel`).

## 6. États & erreurs

- **Vide initial** : « Saisissez un INS pour interroger le DMP. »
- **Chargement** : « Interrogation du DMP… »
- **Erreurs** : 400 (validation), 401 (PIN F05), 502 (« Le DMP a renvoyé une erreur. »).

## 7. Definition of Done

- [ ] US-02.1 → 02.5 · états loading/vide/erreur · PIN intégré · dates FR · tokens M3 · build OK.
