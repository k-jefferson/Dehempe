# Glossaire métier

Vocabulaire à connaître pour spécifier / implémenter le front. Le **vocabulaire technique
(SOAP, XDS, VIHF…) ne doit pas apparaître dans l'UID** ; il sert ici à la compréhension.

| Terme | Définition |
|---|---|
| **DMP** | Dossier Médical Partagé. Le dossier de santé national du patient, hébergé par l'Assurance Maladie. |
| **ANS** | Agence du Numérique en Santé. Définit les référentiels (INS, VIHF, suites cryptographiques, OID…). |
| **CPS** | Carte de Professionnel de Santé. Carte à puce identifiant le praticien. Variante CPS3 = carte actuelle. |
| **CPE** | Carte de Personnel d'Établissement (variante non directement nominative). |
| **RPPS** | Répertoire Partagé des Professionnels de Santé. Identifiant unique du praticien (≈ 11–12 chiffres, dans le `CN` du certificat CPS). |
| **Porteur** | Le praticien titulaire de la carte CPS (nom, prénom, identifiant RPPS, profession). |
| **INS** | Identité Nationale de Santé du patient. Clé d'accès au DMP. |
| **NIR** | INS « principal », 15 chiffres (n° de sécurité sociale). OID `1.2.250.1.213.1.4.8`. |
| **NIA** | INS « d'attente ». OID `1.2.250.1.213.1.4.9`. |
| **INS-C** | INS calculé (historique). Peut apparaître dans les métadonnées patient renvoyées par le DMP. |
| **VIHF** | Vecteur d'Identification et d'Habilitation Formelle. Assertion SAML signée par la CPS, prouvant qui accède et avec quel rôle. Construite **côté API**, invisible pour l'utilisateur. |
| **PIN** | Code confidentiel de la carte CPS. Nécessaire pour les opérations qui signent (appels DMP). Voir `auth-and-pin-flow.md`. |
| **XDS.b** | Profil IHE de partage de documents. Le DMP l'implémente. |
| **ITI-18** | Transaction « Registry Stored Query » : interroger les **métadonnées** des documents. → liste des documents (F03). |
| **ITI-43** | Transaction « Retrieve Document Set » : récupérer le **contenu** d'un document. → visionneuse (F04). |
| **TD 0.2** | Transaction DMP « test d'existence » : le patient a-t-il un DMP ? (F02). |
| **OID** | Identifiant d'objet (ex. `1.2.250.1.213.1.4.8`). Identifie INS, dépôts XDS, communautés… Imposé par l'ANS, jamais inventé. |
| **RepositoryUniqueId** | OID du dépôt XDS où est stocké un document. Nécessaire pour récupérer le contenu (F04). |
| **HomeCommunityId** | OID de la communauté XDS d'origine d'un document (optionnel). |
| **Secteur d'activité** | Code `SAxx` (ex. `SA07` = libéral) lu sur la carte, lié à la structure d'exercice du praticien. |
| **Identifiant_Structure** | Identifiant national de la structure d'exercice (lu sur la carte, utilisé dans le VIHF). Invisible pour l'utilisateur. |

## Codes & valeurs utiles côté UI

- **Statut document** : `APPROVED` (validé, défaut) / `DEPRECATED` (obsolète).
- **classCode / typeCode / formatCode** : nomenclatures de classification des documents (à afficher de façon lisible, cf. F03).
- **genderCode** patient : code sexe renvoyé par le DMP (à mapper en libellé FR).
- **OID INS** : `NIR = 1.2.250.1.213.1.4.8` (défaut), `NIA = 1.2.250.1.213.1.4.9`. Le choix dépend de la **longueur** de l'INS, pas d'un réglage.
