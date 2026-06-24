# Authentification : carte CPS, PIN, clé d'API

L'identité du praticien vient de la **carte CPS physiquement insérée** dans le poste ; l'API
gère toute la cryptographie (mTLS, signature VIHF). Le front n'a que **deux** responsabilités :
1. fournir le **code PIN** quand l'API le réclame ;
2. ajouter une éventuelle **clé d'API** (`X-Api-Key`) si configurée.

## Deux niveaux d'accès à la carte

| Opération | PIN requis ? | Pourquoi |
|---|---|---|
| Lire l'identité de la carte (`GET /api/cps/card`, F01) | **Non** | Lecture d'objets publics de la carte (phase publique PKCS#11). |
| Tout appel DMP (F02, F03, F04) | **Oui** | Nécessite une **signature** par la clé privée de la carte → login PKCS#11 → PIN. |

## Flux PIN (le plus important côté front)

```
Front                         Dehempe.API                      Carte CPS / DMP
  │                                │                                 │
  │ GET /api/patients/{ins}/dmp    │                                 │
  │───────────────────────────────▶│  pas de PIN en session          │
  │                                │  → login PKCS#11 impossible      │
  │   401 + errorCode=CpsPinRequired│                                 │
  │   WWW-Authenticate: CpsPin      │                                 │
  │◀───────────────────────────────│                                 │
  │                                │                                 │
  │ (dialog PIN — saisie masquée)  │                                 │
  │                                │                                 │
  │ GET …/dmp  +  X-Cps-Pin: ****  │                                 │
  │───────────────────────────────▶│  login PKCS#11 (PIN) ──────────▶│
  │                                │  session ouverte (singleton)     │
  │              200 DmpExistence  │◀────────────────────────────────│
  │◀───────────────────────────────│                                 │
  │                                │                                 │
  │ requêtes suivantes (même process API) : PLUS BESOIN du header     │
  │ tant que la carte reste insérée et la session API vivante.        │
```

### Règles d'implémentation (intercepteur HTTP)

1. **Détection** : une réponse `401` est un « PIN requis » **si** `errorCode === "CpsPinRequired"`
   (ou, à défaut, header `WWW-Authenticate` contenant `CpsPin`). Une `401` avec un body
   `{ error: "Clé API…" }` **n'est pas** un PIN requis → ne pas ouvrir le dialog.
2. **Saisie** : ouvrir le dialog PIN (`F05`). Saisie **masquée**, jamais journalisée, jamais
   conservée en clair hors mémoire vive.
3. **Rejeu** : rejouer **la même requête** en ajoutant `X-Cps-Pin: <pin>`.
4. **Mémorisation de session** : après un login réussi, l'API garde la session ouverte ; le front
   peut donc retenir le PIN **en mémoire** (signal/service `PinService`) pour le ré-injecter
   silencieusement sur les requêtes suivantes et **éviter** d'autres 401. À ne **jamais** persister
   (pas de `localStorage`/`sessionStorage`, pas de cookie).
5. **PIN refusé / carte retirée** : si une requête **avec** PIN renvoie de nouveau 401
   CpsPinRequired (ou une erreur d'auth), considérer le PIN invalidé → purger le PIN mémorisé,
   re-proposer la saisie (message « code incorrect ou carte retirée »).
6. **Anti-boucle** : ne tenter le rejeu automatique **qu'une fois** par requête. Si la 2ᵉ tentative
   échoue encore en CpsPinRequired, remonter l'erreur à l'utilisateur (ne pas boucler).
7. **Concurrence** : si plusieurs requêtes partent en parallèle et tombent toutes en 401, n'ouvrir
   **qu'un seul** dialog PIN ; les requêtes en attente réutilisent le PIN saisi (mutualiser la
   promesse/observable de saisie).

### Côté serveur (rappel, pour comprendre)

- Le PIN voyage via l'en-tête **`X-Cps-Pin`** (constante backend `Pkcs11CpsKeyStore.PinHeaderName`).
- En dev local depuis Swagger, un dialog **natif** OS peut s'afficher (`Cps:InteractivePinPrompt`).
  Ce confort de dev est **hors périmètre** du front : côté web, c'est **toujours** le flux header.
- La session PKCS#11 est un **singleton** côté API : un seul login par durée de vie du process.

## Clé d'API (`X-Api-Key`)

- L'API n'exige la clé **que si** `ApiKey:ApiKey` est non vide côté backend (désactivé en dev).
- Le front ajoute `X-Api-Key` **uniquement si** `environment.apiKey` est renseigné.
- En cas de clé absente/invalide alors qu'elle est exigée → `401 { error: "Clé API…" }`
  (à distinguer du PIN, cf. ci-dessus) → message de **configuration**, pas un dialog PIN.

## Sécurité & confidentialité

- Aucune donnée patient ni PIN n'est **persistée** sur le disque par le front.
- Le PIN ne doit **jamais** apparaître dans les logs, l'URL, ni un message d'erreur.
- Les champs de debug (`request`/`response` SOAP, contenant potentiellement des données patient)
  sont **masqués par défaut** dans l'UI (zone repliée « détails techniques »).
- HTTPS de bout en bout (le proxy de dev cible l'endpoint https de l'API).
