# Spécification — Recherche / liste des documents d'un DMP (TD3.1)

> **⚠️ Source de vérité.** Ce fichier est la spécification de référence pour la
> fonctionnalité « lister les documents du DMP d'un patient ». **Toute évolution
> de ce cas d'usage (route, paramètres, filtres, requête XDS, mapping de réponse)
> doit d'abord être décrite/mise à jour ici, puis implémentée.** Voir l'instruction
> dédiée dans `CLAUDE.md` (§ « Recherche de documents (TD3.1) »).

/ **Statut** : implémenté (v1 — approximation `creationTime`)
/ **Transaction DMP** : TD3.1 — *Recherche de documents dans le DMP d'un patient*
/ **Profil IHE** : ITI-18 *Registry Stored Query* (XDS.b), requête stockée `FindDocuments`
/ **Fonctionnalité métier** : `DMP_3.1a` (sélectionner un document dans la liste)

---

## 1. Références normatives

| Document | Emplacement | Sections utiles |
|---|---|---|
| **SEL-MP-037 DMPi v2.10.0 sans MR** | `docs/package dmp/` | §3.5.1.1 `DMP_3.1a` (p.108-112), §3.5.1.3 `TD3.1` (p.114-117) |
| Matrice des droits fonctionnels | `docs/package dmp/PDT-INF-526 …v1.8.pdf` | Droit d'appel TD3.1 selon le profil CPS |
| Exemple de messages | `docs/package dmp/DMP_LPS_Exemple de messages…/TD3.1 - Recherche de document_requête.xml` (+ `…_réponse.xml`) | Format de fil exact (slots, UUID) |
| Code exemple ANS | `docs/package dmp/DMP-LPS-Code-Exemple_C#dotNET…/` | Patterns d'assemblage (VIHF, enveloppe) |

Règles de gestion clés reprises de SEL-MP-037 §3.5.1 :

- **RG_3010** — l'INS du patient peut être passé en paramètre (`patientId`) ; sinon l'INS du VIHF est utilisé.
- **EX_3.1-1030** — la recherche **propose systématiquement les documents actifs** (`availabilityStatus = …StatusType:Approved`). Les documents archivés / masqués / non-visibles / obsolètes sont des filtres **optionnels** que le LPS *peut* proposer.
- **EX_3.1-1055** — le LPS **ne doit pas** filtrer les documents restitués sur le `patientId` (l'INS d'un patient peut changer).
- **EX_3.1-1070** — les dates XDS sont en **UTC** ; l'affichage à l'utilisateur doit se faire en **heure locale** (responsabilité du frontend).
- **EX_3.1-2030** — le LPS **ne doit pas** appeler automatiquement la TD3.1 après une alimentation ; les appels après ouverture du dossier doivent relever d'une demande explicite de l'utilisateur (responsabilité du frontend).

---

## 2. Décisions de conception (et leur justification)

Deux décisions structurantes ont été prises avec le porteur du besoin :

### 2.1. Filtre temporel = `creationTime` (approximation de la « date de soumission »)

Le besoin exprimé est de filtrer **par date de soumission** des documents dans le DMP.

> **Point normatif important (SEL-MP-037 §3.5.1.3, p.116).**
> Dans XDS, **il n'existe aucune requête stockée ni aucun slot permettant de filtrer
> les `DocumentEntry` sur leur date de soumission au Repository.** L'approche conforme
> consiste à **combiner 3 appels** :
> 1. `FindSubmissionSets` avec `$XDSSubmissionSetSubmissionTimeFrom` / `$XDSSubmissionSetSubmissionTimeTo` → renvoie les lots de soumission ;
> 2. `GetAssociations` sur les `entryUUID` des lots → filtrer les associations de type `HasMember` → liste des `targetObject` (documents) ;
> 3. `GetDocuments` avec les `entryUUID` des documents.

**Décision v1 : on n'implémente pas cette combinaison.** On utilise la requête `FindDocuments`
seule et on filtre sur **`$XDSDocumentEntryCreationTimeFrom` / `…To`** (date de **création**
du document), comme **approximation** de la date de soumission.

- ✅ Beaucoup plus simple (1 seul appel SOAP), réutilise l'infrastructure existante.
- ⚠️ **Limite connue** : `creationTime` est la date d'**authoring** du document, pas sa date
  de **soumission** au DMP. Un document créé en janvier mais déposé en juin sera classé
  sur janvier. Pour la plupart des documents (déposés peu après leur création) l'écart est
  négligeable, mais ce n'est **pas** strictement la date de soumission.
- 🔭 **Chemin d'évolution** vers la sémantique exacte : voir §7.

Il existe par ailleurs un 3ᵉ concept temporel distinct dans la spec, la **date d'acte**
(`$XDSDocumentEntryServiceStartTime…` / `ServiceStopTime…`, §3.5.1.3 « …par rapport à une
date d'acte ») — non utilisé ici.

### 2.2. On fait évoluer la route existante (pas de doublon)

La branche `feat/list-documents` contenait déjà une route `GET /api/patients/{ins}/documents`
(scaffolding du commit initial : `FindDocuments` par `creationTime`, statut en paramètre, sans
dates par défaut). **Décision : on fait évoluer cette route** pour la mettre en conformité avec
la présente spec, plutôt que d'ajouter un second endpoint qui listerait aussi des documents.

---

## 3. Contrat d'API

### 3.1. Endpoint

```
GET /api/patients/{ins}/documents
```

### 3.2. Paramètres

| Nom | Emplacement | Type | Obligatoire | Défaut | Description |
|---|---|---|---|---|---|
| `ins` | route | string | ✅ | — | INS du patient. NIR = 15 chiffres. |
| `insOid` | query | string | ❌ | `1.2.250.1.213.1.4.8` (NIR) | OID de l'INS : NIR (`…4.8`) ou NIA (`…4.9`). |
| `createdAfter` | query | date/heure ISO 8601 | ❌ | aucun (*pré-rempli J−30 dans Swagger*) | Borne basse de la fenêtre (date de soumission approximée par `creationTime`, *cf. §2.1*). |
| `createdBefore` | query | date/heure ISO 8601 | ❌ | aucun (*pré-rempli « aujourd'hui » dans Swagger*) | Borne haute de la fenêtre. |
| `classCode` | query | string **répétable** | ❌ | — | Un ou plusieurs codes de classe XDS : `?classCode=x&classCode=y`. |

- **Statut** : **toujours forcé à `Approved`** (`urn:oasis:names:tc:ebxml-regrep:StatusType:Approved`).
  Ce n'est **pas** un paramètre exposé (conforme à EX_3.1-1030, qui exige a minima les documents actifs).
- **Défauts de dates** : **aucun défaut n'est appliqué côté API**. Si `createdAfter` / `createdBefore`
  ne sont pas fournis, **aucun slot temporel n'est envoyé** au DMP (pas de filtre de date). Le
  pré-remplissage J−30 → aujourd'hui est **uniquement** une commodité de Swagger UI
  (`DocumentDateRangeDefaultsOperationFilter`, recalculé à chaque génération du document Swagger) ;
  il n'affecte **pas** les appels directs à l'API.
- Les bornes fournies sont interprétées comme des instants ; elles sont converties en **UTC** puis
  formatées `yyyyMMddHHmmss` pour les slots XDS.

### 3.3. Validation (renvoie `400` via `ValidationBehavior`)

- `ins` non vide ; si `insOid` = NIR → exactement 15 chiffres (`^\d{15}$`).
- `insOid` ∈ { `1.2.250.1.213.1.4.8`, `1.2.250.1.213.1.4.9` }.
- `createdAfter ≤ createdBefore` lorsque les deux sont fournis.

### 3.4. Réponse `200 OK`

`application/json` — enveloppe `DocumentListDto` :

```jsonc
{
  "documents": [
    {
      "uniqueId": "1.2.250.1.999...",          // DocumentEntry.uniqueId (→ TD3.2)
      "repositoryUniqueId": "1.2.250.1.999...",// dépôt XDS (→ TD3.2 / ITI-43)
      "homeCommunityId": "urn:oid:1.2.250...", // optionnel
      "title": "Compte rendu de consultation",
      "status": "Approved",
      "classCode": "...", "typeCode": "...", "formatCode": "...",
      "mimeType": "text/xml",
      "creationTime": "2026-06-01T08:30:00+00:00",
      "serviceStartTime": "...", "serviceStopTime": "...",
      "authorInstitution": "...", "authorPerson": "..."
    }
  ],

  // Diagnostic d'état vide : XML SOAP brut échangé avec le DMP (requête envoyée + réponse
  // reçue). Renseignés UNIQUEMENT quand `documents` est vide ; null dès qu'au moins un
  // document est renvoyé.
  "dmpRequest": null,
  "dmpResponse": null
}
```

> Les dates sont renvoyées en `DateTimeOffset` (UTC) ; **le frontend les convertit en heure
> locale** pour l'affichage (EX_3.1-1070). Le frontend distingue les documents « type patient »
> via `classCode = 90` / `typeCode` commençant par `DOCPAT` (EX_3.1-1040).

### 3.5. Codes d'erreur (mappés par `ExceptionHandlingMiddleware`)

| HTTP | Cas | `errorCode` |
|---|---|---|
| `400` | Validation (INS/OID/plage de dates) | — (`errors` détaillé) |
| `401` | PIN CPS requis (login PKCS#11) → header `WWW-Authenticate: CpsPin` | `CpsPinRequired` |
| `404` | Patient inconnu du DMP | `XDSUnknownPatientId` |
| `502` | Erreur d'authentification / SOAP Fault / erreur registre DMP | code DMP renvoyé |

---

## 4. Conception technique

### 4.1. Flux (Clean Architecture)

```
DocumentsController.GetDocuments (API)
  • emballe le résultat dans DocumentListDto ; si la liste est vide, y joint le XML SOAP
    brut capturé (ISoapRequestCapture) → dmpRequest / dmpResponse
  └─ MediatR → GetDocumentListQuery (Application)
       └─ GetDocumentListQueryHandler
            • parse INS (NIR/NIA)
            • Status = Approved (forcé) ; transmet createdAfter / createdBefore / classCodes tels quels
            └─ IDmpDocumentRepository.FindDocumentsAsync (Domain)
                 └─ DmpDocumentRepository → XdsRegistryClient.FindDocumentsAsync (Infra)
                      • VIHF (CpsVihfContextAccessor) + enveloppe SOAP 1.2 (XdsSoapClientBase)
                      • POST ITI-18 vers Dmp:RegistryEndpoint
                      • slots dates (UTC) + classCode ajoutés seulement s'ils sont fournis
                      • parse les <rim:ExtrinsicObject> → DocumentEntry
```

### 4.2. Mapping paramètres → requête `FindDocuments` (ITI-18)

`AdhocQuery id = urn:uuid:14d4debf-8f97-4251-9a74-a90016b0af0d`, `ResponseOption returnType="LeafClass"`.

| Slot XDS | Source |
|---|---|
| `$XDSDocumentEntryPatientId` | `'{ins}^^^&{oid}&ISO'` |
| `$XDSDocumentEntryStatus` | `('urn:oasis:names:tc:ebxml-regrep:StatusType:Approved')` (forcé) |
| `$XDSDocumentEntryCreationTimeFrom` | `createdAfter` → UTC → `yyyyMMddHHmmss` *(slot omis si absent)* |
| `$XDSDocumentEntryCreationTimeTo` | `createdBefore` → UTC → `yyyyMMddHHmmss` *(slot omis si absent)* |
| `$XDSDocumentEntryClassCode` | `classCode` (répétable) → `('c1','c2')` *(slot omis si aucun code)* |

### 4.3. Fichiers concernés

| Fichier | Rôle |
|---|---|
| `src/Dehempe.API/Controllers/DocumentsController.cs` | route + paramètres `createdAfter`/`createdBefore`/`classCode` ; emballage `DocumentListDto` + capture SOAP |
| `src/Dehempe.API/Swagger/DocumentDateRangeDefaultsOperationFilter.cs` | pré-remplit les défauts de dates (J−30 → aujourd'hui) dans Swagger UI **uniquement** |
| `src/Dehempe.Application/Documents/Queries/GetDocumentListQuery.cs` | query + handler (statut Approved forcé, dates/classCodes transmis) + validator (INS, ordre des dates) |
| `src/Dehempe.Application/Documents/DTOs/DocumentEntryDto.cs` | DTO d'une entrée (inchangé) |
| `src/Dehempe.Application/Documents/DTOs/DocumentListDto.cs` | enveloppe de réponse `{ documents, dmpRequest, dmpResponse }` |
| `src/Dehempe.Application/Common/Interfaces/ISoapRequestCapture.cs`<br>`src/Dehempe.Infrastructure/Dmp/Soap/SoapRequestCapture.cs` | capture scopée du XML SOAP brut (requête + réponse) pour le diagnostic d'état vide |
| `src/Dehempe.Domain/Interfaces/IDmpDocumentRepository.cs` | `DocumentSearchCriteria` (champ `ClassCodes` utilisé) |
| `src/Dehempe.Infrastructure/Dmp/Soap/XdsRegistryClient.cs` | construction `FindDocuments` : slots dates (UTC) + `classCode`, omis si absents |

---

## 5. Exemple

**Requête** — tous les documents actifs (aucun filtre temporel) :
```
GET /api/patients/207058575627097/documents
```
**Requête** — fenêtre explicite :
```
GET /api/patients/207058575627097/documents?createdAfter=2026-01-01&createdBefore=2026-03-31
```
**Requête** — filtre par classe (paramètre répétable) :
```
GET /api/patients/207058575627097/documents?classCode=ABC&classCode=DEF
```

---

## 6. Hors périmètre (v1)

- **Vraie date de soumission** (combinaison `FindSubmissionSets` + `GetAssociations` + `GetDocuments`) — cf. §2.1 et §7.
- **Filtres optionnels EX_3.1-1030** : documents archivés (`…StatusType:Archived` de l'OID asip), masqués (`confidentialityCode = MASQUE_PS`), non-visibles patient/représentants, obsolètes (`Deprecated`). Non exposés : la route renvoie **uniquement** les documents `Approved`.
- **Filtre `typeCode`** (RG_3020) — non exposé en v1. *(Le filtre `classCode` est désormais exposé, cf. §3.2 / §4.2.)*
- **Recherche depuis la dernière connexion** (EX_3.1-1020) — non implémenté.
- **`DMP_3.1b`** (recherche de l'`entryUUID` via `GetDocuments` en `ObjectRef`) — autre cas d'usage, hors de cette route.

---

## 7. Risques & points d'attention

### 7.1. Format du slot `$XDSDocumentEntryPatientId` — ✅ corrigé (validation live requise)

L'exemple **officiel** ANS (`TD3.1 - Recherche de document_requête.xml`) et le kit de référence
(`Dmp.cs`, `XDSb_CVA.cs`) émettent le patientId au format CX :
```xml
<Value>'207058575627097^^^&amp;1.2.250.1.213.1.4.10&amp;ISO^NH'</Value>
```
→ OID **INS DMP `1.2.250.1.213.1.4.10`** (domaine d'identité unifié, **pas** l'OID source NIR `…4.8`
/ NIA `…4.9`), composant CX.5 **`^NH`**, **sans parenthèses** (valeur mono-valuée).

Le scaffolding initial émettait `('{ins}^^^&{oid 4.8/4.9}&ISO')` (OID source + parenthèses de
liste), incohérent avec la resource-id du VIHF (déjà en `…4.10`) → risque de SOAP Fault
`XDSPatientIdDoesNotMatch` ou de résultat vide.

**Correction appliquée** : `XdsRegistryClient.BuildFindDocumentsBody` utilise désormais
`XdsConstants.DmpInsOid` (`…4.10`) + `XdsConstants.PatientIdCx5TypeCode` (`NH`), sans parenthèses.
L'OID est mutualisé dans `XdsConstants.DmpInsOid` (réutilisé par le VIHF et le HL7 V3 GDP, alias
`GdpInsRoot`) pour éviter toute divergence future. ⚠️ **Reste à valider en test live** (carte CPS
+ DMP) : la recherche doit retourner des documents au lieu d'une liste vide.

### 7.2. `creationTime` ≠ date de soumission

Voir §2.1. Documenté comme approximation assumée.

---

## 8. Évolution future — vraie date de soumission

Pour passer à la sémantique exacte (§3.5.1.3, p.116), implémenter la combinaison :

1. `FindSubmissionSets` (`urn:uuid:f26abbcb-ac74-4422-8a30-edb644bbc1a9`) avec
   `$XDSSubmissionSetPatientId`, `$XDSSubmissionSetStatus = Approved`,
   `$XDSSubmissionSetSubmissionTimeFrom` / `…To`.
2. `GetAssociations` (`urn:uuid:a7ae438b-4bc2-4642-93e9-be891f7bb155`) sur les `entryUUID` des
   lots ; ne garder que `AssociationType = …:HasMember` ; collecter les `targetObject`.
3. `GetDocuments` (`urn:uuid:5c4f972b-d56b-40ac-a5fc-c8ca9b40b9d4`) avec les `entryUUID` des
   documents (`$XDSDocumentEntryEntryUUID`, `$XDSDocumentEntryStatus = Approved`).

> ⚠️ Les UUID/slots ci-dessus sont à **confirmer dans `[IHE-TF2A] §3.18`** et l'annexe WSDL
> (`docs/package dmp/annexe-wsdl-schema/`) avant implémentation.

Le paramètre d'API (`createdAfter`/`createdBefore`) resterait inchangé ; seul le mapping interne
(repository + nouveaux clients SOAP) évoluerait. Les nouvelles requêtes stockées et le parsing
des associations devront être ajoutés à `XdsConstants` et à l'infrastructure SOAP.
