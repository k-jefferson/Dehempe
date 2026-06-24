# Design system — Accessibilité & internationalisation

## Langue

- **UI 100% en français** (audience : professionnels de santé français).
- `index.html` : `<html lang="fr">`.
- Vocabulaire métier conforme au `product/glossary.md` ; **pas de jargon technique** exposé
  (SOAP, XDS, VIHF restent internes).

## Formats

| Donnée | Format affiché | Note |
|---|---|---|
| Date (naissance, création doc, validité carte) | `dd/MM/yyyy` | via `DatePipe` locale `fr-FR`. |
| Date+heure | `dd/MM/yyyy HH:mm` | pour les timestamps de documents si pertinent. |
| INS (NIR) | groupé pour lisibilité (ex. `1 23 45 67 890 123 45`) | ne jamais tronquer en silence. |
| RPPS / identifiant praticien | tel quel, lisible | — |
| Sexe (`genderCode`) | libellé FR | mapper le code en « Homme/Femme/… ». |

- Enregistrer la locale `fr` (`registerLocaleData(localeFr)`) et fournir `LOCALE_ID = 'fr-FR'`.

## Accessibilité (cible WCAG 2.1 AA)

- **Contraste** : respecter les tokens M3 (paires `on-*` / `*`) ; vérifier en clair **et** sombre.
  Ne pas créer de couleurs hors thème qui casseraient le contraste.
- **Clavier** : tout est atteignable et actionnable au clavier ; ordre de tabulation logique ;
  focus **visible** (ne pas supprimer l'outline).
- **Focus modale** : le dialog PIN piège le focus (CDK le gère) et le rend au déclencheur à la fermeture.
- **Labels** : chaque champ a un `mat-label` ; chaque icône informative ou `mat-icon-button` a un
  `aria-label` explicite en français.
- **Rôles & live regions** : annoncer les changements asynchrones importants (résultat d'une
  recherche, erreur) via `aria-live` / `LiveAnnouncer` du CDK.
- **Cibles tactiles** : taille minimale confortable (≥ 44px) sur les actions.
- **États non basés uniquement sur la couleur** : un statut (existant / obsolète / erreur) doit
  aussi être porté par une icône et/ou un texte, pas seulement une couleur.

## Données sensibles

- Ne jamais afficher de PIN ; champ masqué (cf. F05).
- Les blocs de debug (XML SOAP `request`/`response`) sont **repliés par défaut** et clairement
  marqués « détails techniques » (peuvent contenir des données patient).
- Pas de donnée patient ni PIN dans les logs navigateur en production.

## Definition of Done « a11y / i18n »

- [ ] Tous les libellés en français.
- [ ] Dates au format `dd/MM/yyyy` (locale fr).
- [ ] Navigable au clavier, focus visible.
- [ ] Icônes/boutons-icônes avec `aria-label`.
- [ ] Contraste OK en clair et sombre.
