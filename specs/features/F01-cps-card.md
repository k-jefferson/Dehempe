# F01 — Lecture de la carte CPS

> **Statut** : 🟡 Spécifié · **Priorité** : Haute · **Dépend de** : — · **Endpoints** : `GET /api/cps/card`

## 1. Objectif

Pour le **praticien**, afficher **l'identité lue sur sa carte CPS insérée** dès le démarrage, afin de
confirmer que la bonne carte est présente et d'instaurer la confiance (c'est « lui » qui accède au DMP).

## 2. Périmètre API

- `GET /api/cps/card` → `CpsCard { porteur, carte }` (cf. [api-contract](../architecture/api-contract.md)).
- Auth : **pas de PIN** (lecture publique de la carte).

## 3. User stories & critères d'acceptation

### US-01.1 — Voir mon identité praticien
**En tant que** praticien, **je veux** voir mon nom, prénom, identifiant (RPPS) et profession **afin de**
vérifier que ma carte est bien lue.
- [ ] Au chargement de l'app, l'app appelle `/api/cps/card` et affiche `porteur.nom`, `porteur.prenom`,
      `porteur.identifiant`, `porteur.profession`.
- [ ] L'identité (nom + profession) est rappelée en permanence dans la toolbar du shell.

### US-01.2 — Voir la validité de ma carte
**En tant que** praticien, **je veux** voir le numéro et les dates de la carte **afin de** anticiper son expiration.
- [ ] Affiche `carte.numero`, `carte.dateEmission`, `carte.dateExpiration` au format `dd/MM/yyyy`.
- [ ] Si `dateExpiration` est passée → badge « Carte expirée ». Si < 30 jours → badge « Expire bientôt ».

### US-01.3 — Aucune carte détectée
**En tant que** praticien, **je veux** un message clair si aucune carte n'est lue **afin de** savoir quoi faire.
- [ ] Sur `502`, afficher « Aucune carte CPS détectée. Insérez votre carte puis réessayez. » + bouton « Réessayer ».

## 4. Règles métier & validations

- Aucune saisie. Lecture seule, sans PIN.
- Le numéro affiché est l'identifiant logique renvoyé par l'API (token CTK / serial), pas le numéro gravé.

## 5. UI / composants (Material 3)

- **Accueil** : `mat-card` « Ma carte CPS » avec lignes label/valeur + badges de validité (`matChip`).
- **Toolbar** : identité compacte (nom + profession) en permanence.
- Icône `badge` / `id_card` (Material Symbols) + `aria-label`.

## 6. États & erreurs

- **Chargement** : « Lecture de la carte CPS… » (`mat-progress-bar`/spinner).
- **Erreur 502** : message « Aucune carte CPS détectée… » + « Réessayer ».

## 7. Definition of Done

- [ ] US-01.1 → 01.3 satisfaites · états loading/erreur · dates `dd/MM/yyyy` · tokens M3 · build OK.
