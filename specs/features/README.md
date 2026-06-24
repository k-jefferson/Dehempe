# Features — index

Une feature = une fiche `Fxx-*.md` (gabarit : `../_templates/feature.template.md`).
Toute nouvelle fonctionnalité **doit** avoir sa fiche avant implémentation.

| # | Feature | Endpoint(s) principal(aux) | PIN | Statut |
|---|---|---|---|---|
| **F01** | [Lecture de la carte CPS](F01-cps-card.md) | `GET /api/cps/card` | Non | 🟡 Spécifié |
| **F02** | [Test d'existence du DMP (TD 0.2)](F02-patient-dmp-existence.md) | `GET /api/patients/{ins}/dmp` | Oui | 🟡 Spécifié |
| **F03** | [Liste des documents (ITI-18)](F03-document-list.md) | `GET /api/patients/{ins}/documents` | Oui | 🟡 Spécifié |
| **F04** | [Consultation d'un document (ITI-43)](F04-document-viewer.md) | `GET …/documents/{uniqueId}/content` | Oui | 🟡 Spécifié |
| **F05** | [Saisie du code PIN (transverse)](F05-pin-entry.md) | en-tête `X-Cps-Pin` (transverse) | — | 🟡 Spécifié |

## Parcours global

```
F01 (identité praticien, au démarrage)
        │
        ▼
F02 (saisie INS → existence DMP) ──── F05 (dialog PIN, déclenché au 1er appel DMP)
        │
        ▼
F03 (liste documents) ───► F04 (consulter / télécharger un document)
```

> État initial du projet : scaffold (thème M3 + shell + couche API) en place ; **aucune** de ces
> features n'est encore implémentée.
