# Contrat d'API — Dehempe.API

> **Source de vérité du backend.** Le front ne doit appeler **que** ces endpoints, avec **ces**
> formes de payload. Tout besoin non couvert ici implique une évolution de `Dehempe.API`
> (à signaler, ne pas simuler côté front).

## Base URL & proxy

- L'API tourne en local : `http://localhost:5012` (http) et `https://localhost:7270` (https).
- `Dehempe.API` fait `UseHttpsRedirection()` et **n'a pas de CORS configuré**.
- **En dev**, le front appelle des URLs **relatives** `/api/...` et un **proxy Angular**
  (`proxy.conf.json`) les redirige vers `https://localhost:7270` (`secure:false`, `changeOrigin:true`).
  → aucun souci CORS, aucune modif backend.
- `environment.apiBaseUrl` vaut `''` en dev (relatif via proxy). En prod, soit le SPA est servi
  par l'API (même origine, `''`), soit on renseigne l'URL absolue de l'API.

## Sérialisation

- JSON **camelCase** (défaut ASP.NET Core). Les `record` C# `Nom`/`Prenom` deviennent `nom`/`prenom`.
- `DateOnly` → chaîne `"yyyy-MM-dd"`. `DateTimeOffset` → ISO 8601 (`"2026-06-23T10:00:00+02:00"`).
- Les booléens « tri-état » côté C# (`bool?`) peuvent être `true` / `false` / `null`.

## Endpoints

### 1. `GET /api/cps/card` — identité de la carte CPS (F01)

- **Auth** : lecture publique de la carte, **pas de PIN**.
- **200** → `CpsCard`
- **502** → carte absente / lecteur / middleware CPS indisponible.

### 2. `GET /api/patients/{ins}/dmp` — test d'existence DMP / TD 0.2 (F02)

- Path : `ins` = NIR (15 chiffres) ou NIA.
- Query : `insOid` (OID de l'INS, défaut `1.2.250.1.213.1.4.8` = NIR ; NIA = `1.2.250.1.213.1.4.9`).
- **Auth** : appel DMP → signature VIHF → **PIN requis** (peut renvoyer **401 CpsPinRequired**, cf. `auth-and-pin-flow.md`).
- **200** → `DmpExistence` · **400** → INS invalide · **502** → erreur DMP.

### 3. `GET /api/patients/{ins}/documents` — liste des documents / ITI-18 (F03)

- Path : `ins`. Query :
  - `insOid` (défaut NIR)
  - `createdAfter`, `createdBefore` : ISO 8601 (filtre date de création)
  - `status` : `APPROVED` (défaut) | `DEPRECATED`
  - `classCode` : **répétable** (un ou plusieurs codes de classe) → `?classCode=x&classCode=y`
- **Auth** : **PIN requis**.
- **200** → `DocumentEntry[]` · **400** → INS invalide · **404** → patient/DMP introuvable.

### 4. `GET /api/patients/{ins}/documents/{uniqueId}/content` — contenu / ITI-43 (F04)

- Path : `ins`, `uniqueId` (**doit être URL-encodé** ; l'API fait `UnescapeDataString`).
- Query : `repositoryUniqueId` (**obligatoire**, OID du dépôt — vient du `DocumentEntry`),
  `homeCommunityId` (optionnel, vient du `DocumentEntry`).
- **Auth** : **PIN requis**.
- **200** → **binaire** ; `Content-Type` = `mimeType` du document (PDF, etc.). À traiter en `responseType: 'blob'`.
- **404** → document introuvable.

> Pour récupérer un contenu (F04), on passe **toujours** par les valeurs `uniqueId` +
> `repositoryUniqueId` (+ `homeCommunityId`) issues de l'entrée renvoyée par la liste (F03).

## Modèles TypeScript (calqués sur les DTOs)

```ts
// core/api/models/cps-card.model.ts
export interface CpsCard { porteur: CpsPorteur; carte: CpsCarte; }
export interface CpsPorteur {
  nom: string;
  prenom: string;
  identifiant: string; // RPPS-like (≈12 chiffres)
  profession: string;
}
export interface CpsCarte {
  numero: string;
  dateEmission: string;   // 'yyyy-MM-dd'
  dateExpiration: string; // 'yyyy-MM-dd'
}

// core/api/models/dmp-existence.model.ts
export interface DmpExistence {
  patientIns: string;
  exists: boolean;
  queryResponseCode: string;
  ackTypeCode: string;
  isAuthorizationValid: boolean | null;
  isAttachedToTreatingPhysician: boolean | null;
  patient: DmpPatient | null;
  errorMessage: string | null;
  request: string | null; // XML SOAP brut — DEBUG seulement, masqué par défaut dans l'UI
}
export interface DmpPatient {
  insC: string | null;
  insNir: string | null;
  status: string | null;
  prefix: string | null;
  givenName: string | null;
  familyName: string | null;
  email: string | null;
  phone: string | null;
  genderCode: string | null;
  birthDate: string | null;          // 'yyyy-MM-dd'
  hasInternetAccount: boolean | null;
  isAttachedToEns: boolean | null;
}

// core/api/models/document-entry.model.ts
export interface DocumentEntry {
  uniqueId: string;
  repositoryUniqueId: string;
  homeCommunityId: string | null;
  title: string | null;
  status: string;                    // 'APPROVED' | 'DEPRECATED'
  classCode: string | null;
  typeCode: string | null;
  formatCode: string | null;
  mimeType: string | null;
  creationTime: string | null;       // ISO 8601
  serviceStartTime: string | null;   // ISO 8601
  serviceStopTime: string | null;    // ISO 8601
  authorInstitution: string | null;
  authorPerson: string | null;
}
```

## Contrat d'erreur

Toutes les erreurs métier passent par `ExceptionHandlingMiddleware` et renvoient du
`application/problem+json`.

### Erreurs DMP (404 / 502 / 401)

```jsonc
{
  "title":      "message lisible",
  "status":     502,
  "errors":     null,
  "errorCode":  "<code DMP>",        // ex: "CpsPinRequired", code SOAP Fault, etc.
  "endpoint":   "https://…",         // debug
  "soapAction": "urn:ihe:…",         // debug
  "request":    "<xml soap…>",        // debug — NE PAS afficher par défaut
  "response":   "<xml soap…>"         // debug — NE PAS afficher par défaut
}
```

### Erreurs de validation (400)

```jsonc
{ "title": "Validation échouée.", "status": 400, "errors": { "ins": ["…"] } }
```

### 401 — deux origines à distinguer

| Origine | Comment la reconnaître | Réaction front |
|---|---|---|
| **PIN CPS requis** | `errorCode === "CpsPinRequired"` (+ header `WWW-Authenticate: CpsPin realm="DMP"`) | Ouvrir le dialog PIN puis **rejouer** la requête avec `X-Cps-Pin` (cf. F05 / `auth-and-pin-flow.md`). |
| **Clé d'API invalide** | body `{ "error": "Clé API manquante ou invalide." }` (pas d'`errorCode`) | Erreur de configuration : message clair, **ne pas** ouvrir le dialog PIN. |

| Code HTTP | Sens générique | Traitement UI |
|---|---|---|
| 400 | Donnée invalide (INS) | Message de validation près du champ. |
| 401 | Voir tableau ci-dessus | PIN ou clé d'API. |
| 404 | Patient / document introuvable | État « vide » dédié, pas une erreur bloquante rouge. |
| 502 | Le DMP a répondu une erreur | Message « le DMP a renvoyé une erreur », détails en mode debug. |
| 500 | Erreur inattendue | Message générique + invitation à réessayer. |

## En-têtes de requête

- `X-Cps-Pin: <pin>` — ajouté **uniquement** après une 401 CpsPinRequired et tant que la session API n'a pas le PIN. Voir `auth-and-pin-flow.md`.
- `X-Api-Key: <clé>` — ajouté **seulement si** `environment.apiKey` est non vide (désactivé en dev).
