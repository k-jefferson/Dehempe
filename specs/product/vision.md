# Vision produit — Déhempé Web

## En une phrase

Déhempé Web est le **poste de consultation du DMP** du praticien : une application web locale,
servie par l'API `Dehempe.API` tournant sur la machine du praticien, qui lui permet de
**consulter le Dossier Médical Partagé** de ses patients en s'authentifiant avec **sa carte CPS**.

## Pourquoi

- L'accès au DMP via IHE XDS.b est technique (SOAP, VIHF signé, mTLS CPS). `Dehempe.API` encapsule
  toute cette complexité. Il manque une **interface humaine** simple par-dessus.
- L'outil est **volontairement découplé de WEDA** : il s'exécute sur le poste du praticien et
  utilise la carte CPS physiquement insérée. C'est un outil autonome.

## Pour qui

**Persona principal — « Dr. Martin », médecin généraliste libéral.**
- Possède une carte CPS3 insérée dans un lecteur sur son poste (macOS ou Windows).
- Veut, en quelques clics, vérifier si un patient a un DMP et consulter ses documents.
- N'est pas technicien : le vocabulaire SOAP/XDS/VIHF doit rester **invisible**.
- Sensible à la confidentialité : il manipule des données de santé (RGPD / référentiels ANS).

Personas secondaires : pharmacien, autre profession de santé porteuse d'une CPS (la spécialité
et le rôle sont lus sur la carte — voir `auth-and-pin-flow.md`).

## Parcours cible (vue d'ensemble)

1. **Démarrage** — l'app détecte la carte CPS insérée et affiche l'identité du praticien (porteur).
2. **Recherche patient** — le praticien saisit l'INS (NIR/NIA) du patient.
3. **PIN** — au premier appel DMP, l'app demande le code PIN de la carte (puis le mémorise pour la session).
4. **Test d'existence** — l'app indique si le patient a un DMP, et quelques infos d'identité.
5. **Documents** — l'app liste les documents du DMP, filtrables, et permet d'en **consulter / télécharger** le contenu.

## Périmètre (v1)

Les capacités exposées par `Dehempe.API` aujourd'hui (voir `architecture/api-contract.md`) :

- ✅ Lire la carte CPS (identité praticien). → **F01**
- ✅ Tester l'existence du DMP d'un patient (TD 0.2). → **F02**
- ✅ Lister les documents d'un patient (ITI-18). → **F03**
- ✅ Récupérer le contenu d'un document (ITI-43). → **F04**
- ✅ Saisir / gérer le code PIN de la carte (transverse). → **F05**

## Non-objectifs (explicitement hors périmètre v1)

- ❌ **Écriture** dans le DMP (alimentation de documents, TD d'envoi). Lecture seule pour l'instant.
- ❌ Gestion multi-praticien / multi-carte simultanée. Une carte insérée = un praticien.
- ❌ Stockage local persistant des données patient (pas de cache disque des contenus DMP).
- ❌ Intégration WEDA. Déhempé reste autonome.
- ❌ Administration / configuration avancée du DMP côté serveur (gérée dans `appsettings*.json`).

## Principes directeurs

1. **Sobriété clinique** — l'écran sert la décision médicale, pas la décoration.
2. **Confiance** — l'origine officielle (DMP / ANS) et l'état d'authentification sont toujours lisibles.
3. **Zéro jargon technique** côté utilisateur (le XML SOAP brut peut rester accessible en mode debug, masqué par défaut).
4. **Cohérence Material 3** — un seul design system, des tokens, aucune dérive visuelle.
5. **Accessibilité** — lisibilité, contraste, navigation clavier (voir `design-system/accessibility-i18n.md`).
