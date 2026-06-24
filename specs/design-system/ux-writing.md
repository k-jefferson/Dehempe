# Design system — UX writing (ton & microcopie)

## Ton

- **Professionnel, sobre, factuel.** Public = soignants pressés. Pas d'humour, pas d'emphase inutile.
- **Clair et orienté action.** Dire quoi faire, pas décrire la technique.
- **Vouvoiement**, phrases courtes, vocabulaire métier du `glossary.md`.
- Jamais de jargon technique exposé (« SOAP », « VIHF », « XDS », « ITI-18 ») dans l'UI utilisateur.

## Boutons & actions

- Verbe à l'infinitif + objet : « Consulter le document », « Télécharger », « Rechercher le DMP »,
  « Valider le code ».
- Éviter « OK » seul : préférer l'action concrète (« Valider », « Fermer », « Réessayer »).

## Champs & aide

- Labels concis : « INS du patient », « Code PIN ».
- Hints utiles : « NIR : 15 chiffres » ; pas de paraphrase inutile.

## Messages d'erreur (gabarit)

> **Ce qui s'est passé** (en clair) + **quoi faire** (action). Pas de code technique en premier plan.

| Situation | Message UI (exemple) |
|---|---|
| INS invalide (400) | « INS invalide. Vérifiez le numéro saisi (NIR : 15 chiffres). » |
| PIN requis (401 CpsPinRequired) | Dialog : « Saisissez le code PIN de votre carte CPS pour accéder au DMP. » |
| PIN incorrect / carte retirée | « Code PIN incorrect ou carte retirée. Réessayez. » |
| Clé d'API invalide (401) | « Accès refusé : clé d'API manquante ou invalide (configuration). » |
| Patient sans DMP / 404 | État vide : « Aucun DMP trouvé pour ce patient. » (pas une erreur rouge) |
| Erreur DMP (502) | « Le DMP a renvoyé une erreur. Réessayez ; si le problème persiste, contactez le support. » |
| Carte CPS absente (502 sur /card) | « Aucune carte CPS détectée. Insérez votre carte puis réessayez. » |
| Erreur inattendue (500) | « Une erreur est survenue. Réessayez. » |

- Détails techniques (endpoint, XML SOAP) → **repliés**, après le message lisible, pour le support.

## États vides

- Titre court + explication + action quand c'est pertinent.
  - 0 document : « Aucun document » / « Ce DMP ne contient aucun document pour les filtres choisis. » + « Réinitialiser les filtres ».
  - Pas encore de recherche : « Saisissez un INS pour interroger le DMP. »

## États de chargement

- Court et honnête : « Lecture de la carte CPS… », « Interrogation du DMP… », « Récupération du document… ».
- Ne pas laisser d'écran blanc : toujours un indicateur + libellé.

## Confidentialité dans la copie

- Ne jamais afficher le PIN ni l'inclure dans un message.
- Mentionner l'origine officielle quand utile pour la confiance (« Données issues du DMP »).
