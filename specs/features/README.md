# Features — index

Une feature = une fiche `Fxx-*.md` (gabarit : `../_templates/feature.template.md`).
Toute nouvelle fonctionnalité **doit** avoir sa fiche avant implémentation.

| # | Feature | Endpoint(s) principal(aux) | PIN | Statut |
|---|---|---|---|---|
| **F01** | [Lecture de la carte CPS](F01-cps-card.md) | `GET /api/cps/card` | Non | 🟡 Spécifié |
| **F02** | [Test d'existence du DMP (TD 0.2)](F02-patient-dmp-existence.md) | `GET /api/patients/{ins}/dmp` | Oui | 🟡 Spécifié |
| **F03** | [Liste des documents (ITI-18)](F03-document-list.md) | `GET /api/patients/{ins}/documents` | Oui | 🔵 En cours |
| **F04** | [Consultation d'un document (ITI-43)](F04-document-viewer.md) | `GET …/documents/{uniqueId}/content` | Oui | 🟡 Spécifié |
| **F05** | [Saisie du code PIN (transverse)](F05-pin-entry.md) | en-tête `X-Cps-Pin` (transverse) | — | 🟡 Spécifié |
| **F06** | [Liste des patients (sélecteur)](F06-patient-list.md) | aucun (données locales) | Non | 🟢 Implémenté |
| **F07** | [Sélection patient → documents](F07-patient-document-navigation.md) | aucun en propre (ouvre F03) | via F03 | 🟢 Implémenté |

## Parcours global

```
F01 (identité praticien, au démarrage)
        │
        ▼
F06 (liste des patients, sidenav — jeu d'essai local)
        │  F07 : clic patient → /patient/:ins/documents
        ▼
F03 (liste documents) ───► F04 (consulter / télécharger un document)
        ▲
        │  (chemin alternatif : saisie manuelle d'INS)
F02 (saisie INS → existence DMP)

F05 (dialog PIN) se déclenche au 1er appel DMP (F02 / F03 / F04).
```

> **F06** alimente le sidenav avec un **jeu d'essai local** (`src/web/src/data/patients-dmp.json`) :
> c'est une feature de navigation/test qui **ne consomme pas** l'API DMP.

> État : **F06** (liste des patients) et **F07** (sélection → documents) sont **implémentées** ; **F03**
> (liste des documents) est **en cours** (liste/tri/pagination/états OK ; filtre `classCode` et ouverture
> F04 à faire). F01, F02, F04, F05 restent à implémenter.
