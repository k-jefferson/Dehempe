# Spécifications — Déhempé Web

Ce dossier est la **source de vérité fonctionnelle et design** de l'application frontend
(`src/web`). Toute évolution du front doit être cohérente avec ces documents.

## À l'attention de Claude Code

> **Avant d'implémenter ou de modifier quoi que ce soit dans `src/web`, lis les specs concernées.**
>
> Ordre de lecture recommandé :
> 1. `architecture/frontend-architecture.md` — structure du code, conventions, patterns.
> 2. `architecture/api-contract.md` — contrat exact de l'API `Dehempe.API`.
> 3. `architecture/auth-and-pin-flow.md` — flux carte CPS / PIN / clé d'API (transverse, **critique**).
> 4. `design-system/*` — règles Material 3 à respecter **systématiquement** (couleurs, typo, composants, a11y, ton).
> 5. La (les) fiche(s) `features/Fxx-*.md` concernée(s) par la demande.
>
> Règles :
> - Ne jamais inventer un endpoint ou un champ : se référer à `api-contract.md`. Si l'API doit
>   évoluer, le signaler explicitement (c'est un changement backend `Dehempe.API`).
> - Toute nouvelle fonctionnalité = une nouvelle fiche `features/Fxx-*.md` (copier `_templates/feature.template.md`).
> - Respecter le design system : pas de couleur/typo/espacement « en dur » hors des tokens M3.
> - UI en **français**, ton défini dans `design-system/ux-writing.md`.

## Structure

```
specs/
├── README.md                       ← ce fichier
├── product/
│   ├── vision.md                   ← pourquoi, pour qui, périmètre / non-objectifs
│   └── glossary.md                 ← DMP, CPS, INS, VIHF, XDS… (vocabulaire métier)
├── architecture/
│   ├── frontend-architecture.md    ← stack, arborescence, état, conventions de code
│   ├── api-contract.md             ← endpoints + DTOs de Dehempe.API (calqués)
│   └── auth-and-pin-flow.md        ← carte CPS, flux PIN (X-Cps-Pin / 401), X-Api-Key
├── design-system/
│   ├── foundations.md              ← thème M3 (couleurs, typo, densité, tokens)
│   ├── components.md               ← quel composant Material pour quel usage
│   ├── layout-navigation.md        ← shell, responsive, en-têtes de page, espacement
│   ├── accessibility-i18n.md       ← a11y WCAG, langue FR, formats date/INS
│   └── ux-writing.md               ← ton, microcopie, messages d'erreur
├── features/
│   ├── README.md                   ← index des features + statut
│   ├── F01-cps-card.md             ← lecture de la carte CPS
│   ├── F02-patient-dmp-existence.md← test d'existence du DMP (TD 0.2)
│   ├── F03-document-list.md        ← liste des documents DMP
│   ├── F04-document-viewer.md      ← consultation / téléchargement d'un document
│   ├── F05-pin-entry.md            ← saisie du code PIN (transverse)
│   └── F06-patient-list.md         ← liste des patients (sélecteur sidenav, jeu d'essai local)
└── _templates/
    └── feature.template.md         ← gabarit d'une nouvelle feature
```

## Statuts utilisés dans les fiches

| Badge | Sens |
|---|---|
| 🟢 Implémenté | Présent et fonctionnel dans `src/web`. |
| 🟡 Spécifié | Décrit ici, **pas encore** implémenté. |
| 🔵 En cours | Implémentation partielle. |
| ⚪ Idée | Pas encore spécifié en détail. |

> État : le **scaffold** (thème M3 + shell de navigation + couche API) est en place. **F06**
> (liste des patients dans le sidenav) est **implémentée** ; les autres features métier (F01–F05)
> restent 🟡 Spécifié.
