# F07 — Sélection d'un patient → ses documents

> **Statut** : 🟢 Implémenté · **Priorité** : Haute · **Dépend de** : F06 (liste / source de l'INS), F03 (écran cible), F05 (PIN) · **Endpoints** : aucun en propre (réutilise ceux de F03)

## 1. Objectif

Pour le **praticien**, **cliquer un patient dans la liste du sidenav** ([F06](F06-patient-list.md)) afin
d'afficher directement, dans la zone de contenu principale, **la liste de ses documents DMP**
([F03](F03-document-list.md)) — sans ressaisir l'INS ni passer par l'écran d'existence ([F02](F02-patient-dmp-existence.md)).

C'est le câblage **sélection → consultation** laissé « à spécifier plus tard » par la v1 de F06
(cf. [F06 §9.1](F06-patient-list.md#9-décisions-actées-validées-avant-implémentation)).

## 2. Périmètre

- **Aucun endpoint propre.** F07 est une **interaction de navigation** : elle ouvre l'écran F03, qui
  interroge l'API (`GET /api/patients/{ins}/documents`, cf. [api-contract](../architecture/api-contract.md)).
- **Route cible** : `patient/:ins/documents` (cf. [frontend-architecture](../architecture/frontend-architecture.md), §Routing).
- **INS** = `matriculeInsNir` du patient sélectionné (jeu d'essai F06), **converti en chaîne** (le JSON
  peut le porter en `number`). `insOid` est déduit par longueur comme en [F02](F02-patient-dmp-existence.md)
  (15 → NIR, sinon NIA) — F07 ne le passe pas, c'est F03 / l'API qui l'appliquent.
- **Auth** : la sélection elle-même ne fait **aucun appel réseau** ; le 1ᵉʳ appel DMP déclenché par F03
  peut ouvrir le dialog **PIN** ([F05](F05-pin-entry.md)).

## 3. User stories & critères d'acceptation

### US-07.1 — Ouvrir les documents d'un patient
**En tant que** praticien, **je veux** cliquer un patient de la liste **afin d'**afficher ses documents dans l'écran principal.
- [ ] Cliquer un élément navigue vers `patient/:ins/documents` ; la zone de contenu affiche l'écran F03 pour ce patient.
- [ ] La sélection au clavier (Entrée/Espace sur l'élément focalisé) produit le même résultat.
- [ ] Aucun appel réseau n'est émis par la sélection elle-même (c'est F03, au chargement de la route, qui interroge le DMP).

### US-07.2 — Voir quel patient est sélectionné
**En tant que** praticien, **je veux** repérer le patient actif dans la liste **afin de** garder le contexte.
- [ ] L'élément du patient courant porte un **état actif** (`routerLinkActive`), distinct du survol, visible en clair **et** sombre.
- [ ] L'état actif persiste tant qu'on consulte les documents (et la visionneuse F04) du même patient.

### US-07.3 — Patient sans INS
**En tant que** praticien, **je veux** comprendre pourquoi certains patients ne sont pas consultables.
- [ ] Si `matriculeInsNir` est `null` (ou vide), l'élément est **non navigable** (désactivé) et porte une
      infobulle « INS indisponible — DMP non consultable ».
- [ ] Le reste de l'affichage F06 (nom, date, couleur de sexe, infobulle JSON) demeure inchangé.

### US-07.4 — Garder le patient en contexte
**En tant que** praticien, **je veux** voir de quel patient proviennent les documents affichés.
- [ ] L'en-tête de l'écran F03 rappelle **nom + prénom + INS** du patient sélectionné (patron
      « en-tête de page », cf. [layout-navigation](../design-system/layout-navigation.md)).
- [ ] Le nom/prénom proviennent du **jeu d'essai F06** (lookup par INS dans `PatientsDataset`) ; F07/F03
      **n'exigent pas** un passage préalable par F02.

### US-07.5 — Mobile : refermer le menu
- [ ] En mode `over` (< `medium`), sélectionner un patient **referme** le sidenav après navigation pour révéler le contenu.

### US-07.6 — Lien direct / rafraîchissement
- [ ] L'URL portant l'INS (`/patient/{ins}/documents`) est **partageable / rechargeable** : un accès
      direct ouvre F03 pour ce patient ; la liste F06 reflète l'état actif si l'INS y figure.

## 4. Règles métier & validations

- **Dérivation de l'INS** : `String(matriculeInsNir)` ; navigation **uniquement** si non `null` / non vide (US-07.3).
- **`insOid`** : non géré par F07 ; déduit par longueur côté F03 / API (cf. F02 / api-contract). Les NIR
  corses (15 caractères avec lettre, ex. `188102B17295165`) restent traités en NIR par la règle de longueur.
- **Pas d'appel réseau à la sélection** : F07 ne fait que router. Les filtres par défaut (J−30 → aujourd'hui,
  statut `APPROVED`) et les états (chargement / vide / erreur) relèvent de **F03**.
- **Contexte patient** : résolu localement via `PatientsDataset` (par INS). Si l'INS n'est pas dans le jeu
  d'essai (accès direct par une URL extérieure), l'en-tête se rabat sur l'INS seul.
- **Remplace** la décision [F06 §9.1](F06-patient-list.md#9-décisions-actées-validées-avant-implémentation)
  (« sélection sans action ») : la sélection **navigue désormais**.

## 5. UI / composants (Material 3)

- **Liste sélectionnable** : le `mat-list` de F06 devient **`mat-nav-list`** ; chaque patient navigable est
  un `<a mat-list-item>` avec `routerLink="/patient/{{ ins }}/documents"` et `routerLinkActive` (item actif).
  Cf. [components](../design-system/components.md) (« `mat-nav-list` … item actif via `routerLinkActive` »).
- **Élément non navigable** (sans INS) : rendu **désactivé** (pas un lien focalisable), `matTooltip`
  explicatif ; conserve l'icône / la couleur de sexe.
- **Écran cible** : F03 inchangé, avec en-tête contextualisé (US-07.4).
- **Aucune nouvelle couleur** : réutiliser les tokens M3 (état actif = tokens de sélection de la liste
  Material) ; pas de hex hors de l'exception `--app-sex-female-*` déjà documentée en F06.

## 6. États & erreurs

- La sélection est **locale** : pas d'état réseau propre. Tous les états (chargement « Interrogation du
  DMP… », vide, 401/PIN, 404, 502) sont ceux de **F03** / **F05**.
- **Patient sans INS** : non navigable (US-07.3) — ce n'est pas une erreur.
- **INS absent du jeu d'essai à l'arrivée directe** : F03 fonctionne quand même (l'en-tête montre l'INS), cf. §4.

## 7. Impacts

- **F06** [Liste des patients](F06-patient-list.md) : `mat-list` → `mat-nav-list`, items `routerLink`, état
  actif ; la décision §9.1 « sélection sans action » est **levée** (renvoi vers F07).
- **F03** [Liste des documents](F03-document-list.md) : devient accessible **directement** par la sélection
  (sans F02) ; en-tête contextualisé par le patient sélectionné (lookup F06).
- **F02** [Existence DMP](F02-patient-dmp-existence.md) : note « Lien avec F06 » mise à jour ; la saisie
  manuelle d'INS reste un **chemin alternatif**, pas le chemin de la sélection.
- **Routing** ([frontend-architecture](../architecture/frontend-architecture.md)) : route
  `patient/:ins/documents` activée (lazy F03).
- **Shell / sidenav** ([layout-navigation](../design-system/layout-navigation.md)) : la liste devient un
  sélecteur de navigation ; fermeture du drawer `over` après sélection.

## 8. Definition of Done

- [ ] US-07.1 → 07.6 satisfaites.
- [ ] Clic **et** clavier ouvrent F03 pour le bon INS ; état actif visible (clair / sombre).
- [ ] Patients sans INS non navigables + infobulle explicative.
- [ ] En-tête F03 rappelle nom / prénom / INS (lookup F06), sans dépendre de F02.
- [ ] Mobile : la sélection referme le sidenav.
- [ ] Tokens M3 uniquement ; FR ; `aria` / focus visible ; `OnPush` ; `npm run build` OK.

## 9. Décisions actées (validées avant implémentation)

1. **Destination = documents (F03) directement** : la sélection ouvre la liste des documents, **sans**
   étape d'existence DMP (F02) intermédiaire.
2. **Source de l'INS** : `matriculeInsNir` du jeu d'essai F06 (stringifié). Patients sans INS : non navigables.
3. **Contexte patient** : nom/prénom récupérés **localement** (jeu d'essai F06) par lookup INS — pas via le DMP.
4. **Sélection F06 § 9.1 levée** : la liste navigue désormais (devient `mat-nav-list` + item actif).
