# F<NN> — <Titre court>

> **Statut** : ⚪ Idée · 🟡 Spécifié · 🔵 En cours · 🟢 Implémenté
> **Priorité** : Haute · Moyenne · Basse
> **Dépend de** : <features / specs liées>
> **Endpoints** : <cf. architecture/api-contract.md>

## 1. Objectif

Pour **<persona>**, permettre de **<faire quoi>** afin de **<valeur métier>**.

## 2. Périmètre API

- `<METHOD> <path>` → `<Modèle>` (cf. [api-contract](../architecture/api-contract.md))
- Auth : PIN requis ? oui/non (cf. [auth-and-pin-flow](../architecture/auth-and-pin-flow.md))

## 3. User stories & critères d'acceptation

### US-<NN>.1 — <titre>
**En tant que** <persona>, **je veux** <action> **afin de** <bénéfice>.

**Critères d'acceptation**
- [ ] Étant donné <contexte>, quand <action>, alors <résultat observable>.
- [ ] …

## 4. Règles métier & validations

- …

## 5. UI / composants (Material 3)

- Écran(s) et navigation.
- Composants Material utilisés (cf. [components](../design-system/components.md)).
- Respecter [foundations](../design-system/foundations.md), [layout](../design-system/layout-navigation.md),
  [ux-writing](../design-system/ux-writing.md).

## 6. États & erreurs

- **Chargement** : …
- **Vide** : …
- **Erreur** : … (cf. ux-writing + contrat d'erreur dans api-contract)

## 7. Definition of Done

- [ ] Toutes les user stories satisfaites.
- [ ] États chargement / vide / erreur traités.
- [ ] a11y + i18n : FR, dates `dd/MM/yyyy`, clavier, `aria-label`.
- [ ] Tokens M3 uniquement (pas de style en dur).
- [ ] `npm run build` OK, `OnPush`.
