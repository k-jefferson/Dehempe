# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Déhempé is a standalone ASP.NET Core 7 API that reads patient records from the French DMP (Dossier Médical Partagé) via IHE XDS.b SOAP, authenticated through the practitioner's CPS smart card. It is intentionally NOT linked to WEDA — it runs on the practitioner's machine and uses the locally inserted CPS card for identity.

## Frontend — Déhempé Web (`src/web`)

The repo also contains **`src/web`**: an **Angular 21 (standalone, zoneless) + Angular Material 3** SPA that calls this API so the practitioner can browse the DMP from a browser. Current state = **scaffold only**: Material 3 theme + responsive navigation shell + a *dormant* API/auth layer; **no business screen is implemented yet** (the home page is a placeholder).

- **Spec-driven — read the specs first.** The **`specs/`** folder (Markdown) is the source of truth for the frontend: product vision, the **API contract** (`specs/architecture/api-contract.md`), the **CPS/PIN auth flow** (`specs/architecture/auth-and-pin-flow.md`), the **Material 3 design system** (`specs/design-system/*`), and one fiche per feature (`specs/features/F0x-*.md`). **Before implementing or modifying anything in `src/web`, read `specs/README.md` and the relevant specs**, and keep them in sync when behaviour changes.
- **Run**: `npm --prefix src/web start` (→ http://localhost:4200). A dev proxy (`src/web/proxy.conf.json`) forwards `/api` to `https://localhost:7270`, so run the API (https profile) too. Preview profile `Déhempé Web (Angular)` is in `.claude/launch.json`. **Build**: `npm --prefix src/web run build`.
- **Conventions** (detail in `specs/architecture/frontend-architecture.md`): standalone components with `OnPush` + Signals, functional HTTP interceptors, the **PIN flow** (`401 CpsPinRequired` → `X-Cps-Pin` replay) under `src/web/src/app/core/auth/`, **Material 3 tokens only** (palette generated from seed `#1565C0` in `src/web/src/styles/_theme-colors.scss`). UI in **French**, like the logs.

## Commands

Build / run / test target the solution `Dehempe.sln`:

```bash
dotnet build Dehempe.sln                                  # build everything
dotnet build src/Dehempe.API -c Release                   # API only, release config
dotnet run --project src/Dehempe.API                      # run the API (port 5012 http, 7270 https)
dotnet test Dehempe.sln                                   # run all tests
dotnet test tests/Dehempe.Application.Tests               # one test project
dotnet test --filter "FullyQualifiedName~MyTest"          # one test by name
```

Two preview-server profiles are declared in `.claude/launch.json` (`Dehempe API (http)` and `Dehempe API (https)`) — prefer `preview_start` over `dotnet run` so logs are streamed.

Swagger is mounted at `/swagger` in `Development`.

### Test suite status

The three `tests/Dehempe.*.Tests/` projects contain only the `UnitTest1.cs` xUnit scaffold — there are **no real tests yet**. `dotnet test` exits green but validates nothing. Do not infer working behaviour from a green test run.

### macOS debug helpers

When `/api/cps/card` returns empty/wrong data, these CLI checks reproduce what the macOS provider does:

```bash
security list-smartcards                       # must print fr.asip.esante.CPSToken:<id>
swift /tmp/find_cps.swift                      # ad-hoc SecItemCopyMatching probe
# (see MacOsCtkTokenCertificateProvider for the exact Swift script it runs)
```

## Architecture

Strict Clean Architecture dependency direction — **inner layers never reference outer layers**:

```
Domain  ←  Application  ←  Infrastructure  ←  API
```

- **Dehempe.Domain** — pure types only. Value objects (`Ins`, `DocumentUniqueId`, `RepositoryUniqueId`), entities (`DocumentEntry`, `DocumentContent`), the `IDmpDocumentRepository` interface, and the `DmpException` hierarchy. No framework code.
- **Dehempe.Application** — CQRS via MediatR 11. Each handler lives next to its `IRequest` record under `Cps/Queries/` or `Documents/Queries/`. The `ValidationBehavior<,>` pipeline runs FluentValidation before every handler. DTOs are records.
- **Dehempe.Infrastructure** — every concrete adapter:
  - `Dmp/Soap/` — XDS.b SOAP clients (`XdsRegistryClient` = ITI-18, `XdsRepositoryClient` = ITI-43), hand-built SOAP envelopes (no WCF generated code). All constants live in `XdsConstants.cs`.
  - `Dmp/Auth/` — `CpsAuthService` loads the X.509 cert, `VihfService` builds & XML-signs the SAML 2.0 VIHF assertion injected as WS-Security header. `CpsVihfContextAccessor` derives the practitioner identity from the CPS cert and the patient INS from the HTTP route.
  - `Dmp/Card/` — exposes the inserted CPS card via `ICpsCardReaderService`. See "CPS card access" below.
- **Dehempe.API** — controllers, two middlewares (`ExceptionHandlingMiddleware` maps `DmpException`/`ValidationException` to HTTP status codes; `ApiKeyMiddleware` enforces `X-Api-Key` header *only when* `ApiKey:ApiKey` is non-empty).

MediatR + FluentValidation are wired in `Dehempe.Application/DependencyInjection.cs` (`AddApplication()`). All Infrastructure services and HttpClients are wired in `Dehempe.Infrastructure/DependencyInjection/InfrastructureServiceExtensions.cs` (`AddInfrastructure(IConfiguration)`).

## CPS card access — platform-specific quirks

`CpsCardReaderService` picks a certificate provider at runtime based on `RuntimeInformation.IsOSPlatform(OSPlatform.OSX)`:

- **Windows** → `KeychainCertificateProvider`. The CPS middleware (eGate / Cryptolib) publishes the personal cert into `X509Store(CurrentUser\My)`, filterable by issuer (`IGC-SANTE`, `ASIPSANTE`, etc.). Standard `X509Store` is enough.
- **macOS** → `MacOsCtkTokenCertificateProvider`. CryptoTokenKit owns the card (`fr.asip.esante.CPSToken:<serial>`) and **does not** publish certs into the standard keychain. The provider:
  1. Shells out to `security list-smartcards` to enumerate CTK token IDs.
  2. For each token, spawns `swift -` with an inline script that calls `SecItemCopyMatching` with `kSecAttrTokenID = "<tokenId>"` and prints base64 DER certs.
  3. Filters to the personal cert (presence of SN + GN in the Subject).

Direct PC/SC APDU access to the card from .NET **does not work on macOS** while the CryptoTokenKit extension is loaded — it owns the protocol session and `SCardTransmit` returns `NotTransacted`. Do not try to reintroduce `IsoReader`/raw APDU on macOS without a strategy to coexist with CTK.

CPS3 personal certificate Subject DN uses a multi-AVA RDN that .NET's `X500RelativeDistinguishedName.GetSingleElementType()` **refuses** with `InvalidOperationException`. The Subject is parsed directly from `RawData` using `System.Formats.Asn1.AsnReader` in `CpsCardReaderService.ParseSubject` — preserve this approach when touching that file.

Field mapping for CPS3 personal certs:
- `SN` (OID 2.5.4.4) → `Porteur.Nom`
- `GN` (OID 2.5.4.42) → `Porteur.Prenom`
- `CN` (OID 2.5.4.3) → `Porteur.Identifiant` (12-digit RPPS-like)
- `title` (OID 2.5.4.12) → `Porteur.Profession`
- Cert `NotBefore` / `NotAfter` → `Carte.DateEmission` / `Carte.DateExpiration`
- CTK token id (macOS) or cert serial (Windows) → `Carte.Numero`. **Not** the hardware card number printed on the card — that's only reachable via APDU and CTK blocks us.

Two parsers coexist for CPS certs and they are **not interchangeable**:
- `Dmp/Auth/CpsCertificateParser.cs` — used by `CpsVihfContextAccessor` to derive the practitioner identity for VIHF assertions. Assumes the classic CPS3 DN shape (`CN=NOM PRENOM, SERIALNUMBER=…, OU=ROLE`).
- The inline `ParseSubject` in `CpsCardReaderService` — used by `/api/cps/card`. Walks the DER via `AsnReader` to handle the multi-AVA Subject (`title`/`GN`/`SN`/`CN`) of newer CPS3 certs.

The CTK/Swift provider (`MacOsCtkTokenCertificateProvider`) is now a **transitional fallback only**. The primary path for VIHF signing AND speciality extraction is PKCS#11 (see next section). Do not invest in CTK unless PKCS#11 stops working.

## Paires de clés CPS3 — auth vs signature (NE PAS confondre)

Une carte CPS3 personnelle porte **deux paires (cert, clé)** distinctes et leur usage
est strict :

| Usage | `CKA_ID` (dernier octet) | À utiliser pour | À NE PAS utiliser pour |
|---|---|---|---|
| **Authentification** | `0x20` | Handshake mTLS / `ClientCertVerify` | Signature applicative |
| **Signature** | `0x10` | Assertion VIHF, signature de documents | Handshake mTLS |

Les deux certs portent le **même DN** (même praticien), mais des `KeyUsage` X.509 différents
(`digitalSignature, keyEncipherment` pour auth ; `digitalSignature, nonRepudiation` pour signature).
Le DMP vérifie cette distinction côté serveur — utiliser le cert d'auth pour signer le VIHF
fait remonter le SOAP Fault :

```
soap:Sender / soap:DMPInvalidCertificate
Le certificat ayant signé le VIHF est invalide : Not a valid signature certificate
```

**Implémentation** :
- `Pkcs11CpsKeyStore` expose `GetAuthCertificate()` / `SignWithAuthKey(...)` ET
  `GetSignatureCertificate()` / `SignWithSignatureKey(...)`. Les deux paires sont
  recherchées en phase publique par `FindCertificateByCkaIdSuffix(0x20|0x10, ...)`,
  les clés privées en phase post-login par `FindPrivateKeyByCkaId(...)`.
- `ICpsAuthService` a 4 méthodes : `GetAuthenticationCertificateAsync` / `GetAuthenticationKeyAsync`
  (mTLS) et `GetSignatureCertificateAsync` / `GetSignatureKeyAsync` (VIHF, docs).
- `VihfService.BuildVihfAssertionAsync` appelle **uniquement** les méthodes signature.
- `InfrastructureServiceExtensions.BuildClientHandler` attache **uniquement** le cert d'auth
  au `HttpClientHandler.ClientCertificates` pour le mTLS direct (Windows / Linux non-tunnel).
- `Pkcs11RsaKey` prend un délégué `Func<byte[], byte[]>` en constructeur (pas un appel
  hardcodé) pour pouvoir wrapper l'une ou l'autre clé.

**Quand modifier ces fichiers** : ne **jamais** passer le cert/clé d'auth à `VihfService`
ni au signataire de documents. Ne **jamais** passer le cert/clé de signature au handshake
mTLS. Si tu ajoutes un nouveau service de signature (XAdES, CMS, etc.), il doit prendre
ses dépendances via `GetSignature*Async` — pas via `GetAuthentication*Async`.

**Fallback sans PKCS#11** (`.p12`, magasin système, CTK macOS legacy) : on ne dispose que
d'UNE paire, donc `CpsAuthService` retourne la même paire pour les deux usages avec un
warning explicite. Suffisant en dev avec un `.p12` ANS combiné ; **insuffisant en prod** sur
carte CPS3 réelle — PKCS#11 est alors obligatoire.

## CPS via PKCS#11 — pipeline complet

C'est le chemin principal pour tout ce qui nécessite la **clé privée** ou des **données privées** de la carte CPS (signature VIHF, mTLS, etc.). Implémenté dans `Dehempe.Infrastructure/Dmp/Auth/Pkcs11/`.

### Auto-détection de la librairie

`Pkcs11CpsKeyStore.ResolveLibraryPath` sonde les emplacements connus par plateforme dans cet ordre :

- **macOS** : `/usr/local/lib/libcps3_pkcs11_osx.dylib`, puis `/Library/Frameworks/cps3.framework/cps3_pkcs11`
- **Windows** : `C:\Windows\System32\cps3_pkcs11_w64.dll`, puis `C:\Program Files\santeestoolbox\cps3_pkcs11_w64.dll`, puis `C:\Windows\System32\cps3_pkcs11.dll`
- **Linux** : `/usr/lib/libcps3_pkcs11.so`, puis `/usr/local/lib/libcps3_pkcs11.so`

`Cps:Pkcs11LibraryPath` n'est qu'un **override** optionnel pour forcer un chemin. Ne pas le renseigner en config sauf cas spécifique — l'auto-détection couvre les installs ANS standards.

### Init en deux phases

`Pkcs11CpsKeyStore` est **singleton** dans la DI. Sa session reste ouverte pour toute la vie du process. Init découpée pour ne demander le PIN qu'au dernier moment :

1. **Phase publique** (`EnsureLibraryLoaded`, sans PIN) — charge la lib, ouvre la session, trouve les **DEUX certificats** (auth `CKA_ID` se terminant par `0x20` et signature par `0x10` — voir section précédente), mémorise leurs `CKA_ID` pour appariement ultérieur. Lit aussi les objets `CKO_DATA` publics (`CKA_PRIVATE=false`), dont `CPS_INFO_PS`.
2. **Phase login** (`EnsureLoggedIn`, PIN requis) — déclenchée par `SignWithAuthKey` / `SignWithSignatureKey` à la première signature uniquement. Recherche ensuite les **DEUX clés privées** correspondantes. Les `CKO_PRIVATE_KEY` portent `CKA_PRIVATE=true` et sont **invisibles tant qu'on n'est pas logué** — d'où la séparation.

**Ne JAMAIS** rechercher `CKO_PRIVATE_KEY` dans la phase publique : l'appel renvoie 0 objet sans erreur, ce qui produit un message d'erreur trompeur. Le test fait passer le build, pas le runtime.

### Lecture de la spécialité — objet `CPS_INFO_PS`

Le code spécialité ANS (ex: `SM26` = médecine générale, table R01 / OID `1.2.250.1.71.4.2.5`) **n'est pas dans le certificat X.509**. Il est dans un objet de données PKCS#11 public sur la carte, lu par `Pkcs11CpsKeyStore.ReadSpecialityCode()` :

```
CKA_CLASS   = CKO_DATA
CKA_TOKEN   = true
CKA_PRIVATE = false        ← public, AUCUN PIN requis
CKA_LABEL   = "CPS_INFO_PS"
```

Convention CPS3 confirmée par le code exemple ANS (`PsSignatureProvider.getCodeSpecialite`) : **le code spécialité occupe les 4 derniers caractères** de la valeur UTF-8 brute (qui contient aussi RPPS/Nom/Prénom en clair, séparés par des octets de contrôle BER).

Le code est injecté dans `VihfContext.PractitionerSpecialityCode` par `CpsVihfContextAccessor` (cache local — lecture une seule fois par durée de vie de la requête HTTP) et utilisé dans le VIHF (voir section suivante).

### Structure d'exercice & secteur — objets `CPS_ACTIVITY_xx_PS` (LUS SUR LA CARTE, PAS EN CONFIG)

**Règle DMP critique** : en authentification **directe** par CPS, le champ `Identifiant_Structure` du VIHF doit être le `Struct_IdNat` **lu sur la carte** (SEL-MP-037 §VIHF, p.156 : « Pour les CPS hors remplaçant : Struct_IdNat de la CPS »). Le mettre en config produit l'erreur DMP **« Impossible d'effectuer un test d'existence : Structure introuvable ou Inactive »** (ou « PS et Structure non liés »). Le DMP vérifie en plus la **cohérence** structure ↔ secteur ↔ carte : les deux doivent venir du **même exercice**.

Un praticien a une ou plusieurs **situations d'exercice**, une par objet `CPS_ACTIVITY_01_PS` … `CPS_ACTIVITY_15_PS`. Ces objets sont **`CKA_PRIVATE=true`** → login requis (donc PIN). Format BER-TLV, conteneur `0xEE` englobant :

```
0x84 (len)  raison sociale     ex: "CABINET M DOC0073574"
0x85 (len)  Struct_IdNat       ex: "499700735741008"   ← Identifiant_Structure du VIHF
0x86 (len=4) secteur d'activité ex: "SA07"             ← (le getSecteurActivite ANS prend ces 4 derniers chars)
```

`Pkcs11CpsKeyStore.ReadActivities()` lit et décode tous les exercices (cache process). `CpsVihfContextAccessor.ResolveStructure()` **sélectionne l'exercice dont le secteur correspond** à `Dmp:OrganizationSector` (sélecteur, ex: `SA07` = libéral), puis dérive `Identifiant_Structure` ET `Secteur_Activite` de CE MÊME exercice → cohérents par construction. Exemple réel de la carte de test : exercice 1 = `HOPITAL GENERIQUE` / `10B0307377` / `SA01`, exercice 2 = `CABINET M…` / `499700735741008` / `SA07`.

**Ne JAMAIS** remettre `Identifiant_Structure` en dur dans la config pour le chemin PKCS#11. `Cps:OrganizationId` n'est plus qu'un **fallback** pour le mode dégradé sans PKCS#11 (.p12). `Dmp:OrganizationSector` n'est plus la valeur émise telle quelle mais le **sélecteur de secteur** (on en extrait le code `SAxx` avant le `^`).

### Format du `<Role>` dans le VIHF (très strict)

Le DMP rejette le VIHF si le rôle n'est pas envoyé comme **deux** `<AttributeValue>` distincts dans le même `<Attribute>`, avec ces nomenclatures précises :

```xml
<saml2:Attribute Name="urn:oasis:names:tc:xacml:2.0:subject:role">
  <saml2:AttributeValue>
    <Role code="10"   codeSystem="1.2.250.1.71.1.2.7" codeSystemName="G15"
          displayName="Médecin" xsi:type="CE" xmlns="urn:hl7-org:v3" />
  </saml2:AttributeValue>
  <saml2:AttributeValue>
    <Role code="SM26" codeSystem="1.2.250.1.71.4.2.5" codeSystemName="R01"
          displayName="..." xsi:type="CE" xmlns="urn:hl7-org:v3" />
  </saml2:AttributeValue>
</saml2:Attribute>
```

- 1ʳᵉ valeur = **profession** (G15, OID `1.2.250.1.71.1.2.7`). `codeSystemName="G15"` est obligatoire.
- 2ᵉ valeur = **spécialité** (R01, OID `1.2.250.1.71.4.2.5`). **Obligatoire pour Médecins et Pharmaciens** — sans elle, le DMP répond *« Aucune spécialité dans le vihf. Spécialité obligatoire pour les Médecins... »*.

L'implémentation est dans `VihfService.AddRoleAttr` ; ne pas régresser sur ces deux points.

### Code PIN — flux frontend → API

Le middleware ANS sur macOS **n'expose pas** `CKF_PROTECTED_AUTHENTICATION_PATH`, donc PKCS#11 ne peut pas déclencher de dialog natif. L'API est un service backend, elle ne peut pas non plus afficher de fenêtre. Le PIN **doit** donc venir du frontend via le header HTTP **`X-Cps-Pin`** (constante `Pkcs11CpsKeyStore.PinHeaderName`).

Cycle de vie attendu côté client :

1. Première requête sans header → l'API tente le login → `DmpPinRequiredException` → réponse **`401 Unauthorized`** avec `errorCode = "CpsPinRequired"` et `WWW-Authenticate: CpsPin realm="DMP"`.
2. Le frontend détecte ce code, affiche son dialog de saisie, et **rejoue** la requête avec `X-Cps-Pin: <pin>`.
3. Login PKCS#11 effectué, **la session reste ouverte** (singleton). Les requêtes suivantes au même process n'ont plus besoin du header tant que la carte reste insérée.
4. Si la carte est arrachée puis réinsérée → la session devient invalide ; le keystore devra rouvrir et reloguer (non géré actuellement, à surveiller).

Côté .NET, `Pkcs11CpsKeyStore` lit le header via `IHttpContextAccessor`. Il accepte un `Cps:Pkcs11Pin` en fallback dev **uniquement** — ce champ NE DOIT PAS être renseigné en production (il bypasse complètement le flux frontend).

`DmpPinRequiredException` est mappée par `ExceptionHandlingMiddleware` en `401`. Toute nouvelle exception d'auth doit s'aligner sur ce contrat pour rester compatible avec le frontend.

**Ordre de résolution du PIN** dans `Pkcs11CpsKeyStore.Login()` : (1) header `X-Cps-Pin`, (2) `Cps:Pkcs11Pin` (fallback dev), (3) **dialog natif** si `Cps:InteractivePinPrompt = true`, sinon (4) `DmpPinRequiredException` → 401.

### Dialog PIN natif (test local / Swagger)

Pour tester l'API depuis Swagger sur le poste du praticien (pas de frontend pour fournir le header), activer `Cps:InteractivePinPrompt = true`. Quand le header est absent, `NativePinPrompt.TryPrompt` affiche un dialog OS à saisie masquée — **osascript** (`display dialog … with hidden answer`) sur macOS, **WinForms via PowerShell** sur Windows — et le PIN saisi est utilisé pour le login. Annulation / timeout (3 min) / plateforme non supportée → on retombe sur le `401 CpsPinRequired`.

- C'est un confort de **dev** : laisser `false` en production (le frontend gère la saisie via le header, contrat 401 inchangé). Activé dans `appsettings.Development.json`.
- Swagger expose en plus un champ d'en-tête optionnel `X-Cps-Pin` sur chaque opération (`CpsPinHeaderOperationFilter`) : on peut saisir le PIN directement dans Swagger au lieu d'attendre le dialog.
- Le dialog s'affiche bien depuis le process Kestrel (session GUI de l'utilisateur). Il **bloque** le thread de la requête le temps de la saisie — acceptable pour une API locale mono-utilisateur.

## Tunnel mTLS local (macOS dev) — démarrage automatique

Sur macOS dev, .NET ne peut pas attacher un cert client PKCS#11 au handshake TLS
(limitation `AppleCertificatePal.CopyWithPrivateKey`). On contourne avec `stunnel`
qui termine le mTLS côté carte et expose un endpoint HTTP loopback (cf.
`docs/stunnel/README.md` pour le détail). Quand `Dmp:TunnelEndpoint` est renseigné :

1. `StunnelManager` (singleton DI, `Dmp/Soap/StunnelManager.cs`) est instancié.
2. À la première requête XDS sortante, `DmpTunnelHandler.SendAsync` appelle
   `EnsureRunningAsync` AVANT de réécrire l'URL :
   - Test TCP rapide (`TcpClient.ConnectAsync` 500 ms) sur `Dmp:TunnelEndpoint`.
   - Si le port est fermé → exécute `docs/stunnel/start-tunnel.sh` via
     `/bin/bash` en sous-process, attend que le port LISTEN apparaisse (max 15 s).
3. Les requêtes suivantes court-circuitent le check (`_verified = true`).

**L'utilisateur n'a plus à lancer stunnel manuellement** — l'API s'en occupe.
Si stunnel meurt en cours de route, le manager ne ré-évalue pas automatiquement
(évite un cycle infini lance/crash si la conf est cassée) ; un redémarrage de
l'API ou un appel à `StunnelManager.Invalidate()` est nécessaire pour rouvrir
le tunnel.

Auto-détection du script : `StunnelManager` remonte depuis `AppContext.BaseDirectory`
en cherchant `docs/stunnel/start-tunnel.sh`. Override via `Dmp:StunnelStartScript`
si le binaire .NET est exécuté hors du repo source.

**Quand modifier `DmpTunnelHandler` ou `StunnelManager`** : ne pas retirer l'appel
`EnsureRunningAsync` du handler — sinon les utilisateurs auront `Connection refused`
au premier appel après un reboot.

## DMP SOAP conventions

Every outbound XDS request is built by `XdsSoapClientBase`:
1. The handler resolves the patient INS from the route and the practitioner identity from the CPS cert via `IVihfContextAccessor`.
2. `VihfService.BuildVihfAssertionAsync` produces a SAML 2.0 assertion, XML-signed (SHA-256 RSA) with the CPS private key.
3. The assertion is injected as a WS-Security header on a SOAP 1.2 envelope; `SOAPAction` matches the ITI URI in `XdsConstants`.
4. Errors are detected by inspecting the `<RegistryResponse>` status and any `<RegistryError>` codes — translate these to `DmpPatientNotFoundException` / `DmpDocumentNotFoundException` / `DmpException` for `ExceptionHandlingMiddleware` to map.

INS OIDs: NIR (15 digits) = `1.2.250.1.213.1.4.8`, NIA = `1.2.250.1.213.1.4.9`. The choice is driven by `Ins` value length, not by configuration.

`RepositoryUniqueId` and `HomeCommunityId` in `appsettings*.json` are **OIDs imposed by the ANS** for the targeted DMP environment (test or prod). They are not free identifiers to invent — when in doubt, copy from the ANS integration guide rather than generating new values.

## ANS reference documentation

The `docs/package dmp/` folder holds the official Assurance Maladie documentation set — committed in the repo. **Always consult it before implementing or modifying any DMP feature** rather than guessing from sample code or memory:

### Format CDA R2 (CI-SIS)

The `docs/CI-SIS/` folder holds the CI-SIS interoperability framework documentation. **Always consult it before implementing or modifying any feature that involves reading, displaying, or processing CDA documents** (F04 document viewer, future structured content extraction, etc.):

- **`CI-SIS_CONTENU_VOLET-STRUCTURATION-MINIMALE_V1.16.8.pdf`** — defines the minimal structuring profile for CDA R2 documents exchanged via the DMP. All documents returned by the DMP (`GET .../documents/{uniqueId}/content`) are CDA R2:
  - **Niveau 1** : non-structured content; the CDA wraps a non-XML body (typically a base64-encoded PDF in `<nonXMLBody>`).
  - **Niveau 3** : structured content; the CDA body is XML (`<structuredBody>`) with coded sections.
  - The ANS publishes a reference XSL stylesheet for rendering N3 CDA documents in a browser — use it when implementing the document viewer (F04).

**When working on F04 or any CDA-related feature**, read this spec before writing code. Do not attempt to parse or render CDA content from memory or assumptions.

- **`SEL-MP-037 DMPi v2.10.0 sans MR.pdf`** — the main DMPi specification (DMP interface). The authoritative source for transactions, message shapes, and business rules. The `avec MR` variant is the same spec annotated with revision marks.
- **`PDT-INF-526 - Matrice des droits fonctionnels v1.8.pdf`** — which transaction is allowed for which CPS profile / access context (normal / urgence / bris-de-glace). Check this before adding a route that calls a new TD.
- **`PDT-INF-527 - Accès Web au système DMP v1.7.pdf`** — web access flows, useful for cross-referencing the SOAP equivalents.
- **`PDT-INF-579v4 — Référentiel des suites cryptographiques`** — accepted TLS suites + signature algorithms for VIHF. Reference when touching `VihfService` or HTTPS configuration.
- **`PDT-INF-606 — Matrice des droits fonctionnels AIR`** — AIR (Accès Inopiné Restreint) variant of the rights matrix.
- **`PDT-INF-617 — CPE non directement nominatives IGC SANTE`** — CPE card variant of CPS.
- **`DMP-LPS-Code-Exemple_C#dotNET-v2.06.01/`** — the official C#/WCF reference implementation by ANS. Use it as inspiration when implementing a new transaction (especially `AssertionLibrary/` for VIHF wiring), but do **not** copy WCF-generated code verbatim — this project hand-builds SOAP envelopes (see `XdsSoapClientBase`) and that constraint stays.
- **`DMP_LPS_Exemple de messages_1.0.1_1 - v02.05.00/`** — real request/response payload examples for each TD. The fastest way to verify the exact XML shape expected by the DMP (slot names, classification UUIDs, etc.).
- **`annexe-wsdl-schema/wsdl/`** — XDS.b, PatientsSpecific, patientCertif, PDQ WSDLs. Use these to read the contract of any operation; do not regenerate WCF clients from them.

When asked to implement a new DMP transaction (TD-x.y), the canonical workflow is:
1. Locate the TD in `SEL-MP-037` for the business rules and required fields.
2. Find a matching request/response pair in `DMP_LPS_Exemple de messages/` to confirm wire format.
3. Cross-check the matrix in `PDT-INF-526` to know which CPS profile may invoke it.
4. Look at `DMP-LPS-Code-Exemple_C#dotNET-v2.06.01/` for an implementation pattern, then translate to this project's hand-built SOAP style.

## Specs fonctionnelles (`specs/`) — source de vérité par cas d'usage

Certaines fonctionnalités ont une **spécification fonctionnelle et technique dédiée** dans le
dossier `specs/`. **Ces fichiers font autorité** : avant de travailler sur le cas d'usage
correspondant, **lis la spec ; et pour toute évolution, mets d'abord la spec à jour, puis le
code** (les deux doivent rester cohérents). C'est ce qui garantit la consistance dans le temps,
quel que soit le prompt ou la session.

| Cas d'usage | Spec | Transaction | Route |
|---|---|---|---|
| Recherche / liste des documents d'un patient | [`specs/recherche-documents.md`](specs/recherche-documents.md) | TD3.1 (ITI-18 `FindDocuments`) | `GET /api/patients/{ins}/documents` |

Décisions actées pour la **recherche de documents (TD3.1)**, à ne pas régresser sans relire la spec :
- filtre temporel `dateDebut`/`dateFin` mappé sur `creationTime` XDS (**approximation** de la date
  de soumission — la vraie date de soumission imposerait la combinaison `FindSubmissionSets` +
  `GetAssociations` + `GetDocuments`, cf. SEL-MP-037 §3.5.1.3) ;
- défauts : `dateDebut` = aujourd'hui − 30 j, `dateFin` = aujourd'hui ;
- statut **toujours forcé à `Approved`** (non paramétrable).

## Config

Layered via the standard ASP.NET Core mechanism. **Tous les `src/Dehempe.API/appsettings*.json` sont gitignored** car ils contiennent des données sensibles (OID de structure / FINESS, endpoints DMP par environnement, éventuellement un PIN de dev). Le `.gitignore` ne laisse passer qu'un futur `appsettings.template.json` (sans secret). À l'installation, chaque poste doit reconstituer ses fichiers `appsettings.json`, `appsettings.Development.json` et éventuellement `appsettings.Local.json` localement — soit à la main, soit via env vars (préfixe ASP.NET Core standard : `Dmp__RegistryEndpoint`, `Cps__OrganizationId`, etc.).

Key sections:
- `Cps` — `CertificatePath` + `CertificatePassword` (.p12) OR `CertificateThumbprint` (store lookup). **`OrganizationId`, `Pkcs11LibraryPath` and `Pkcs11Pin` doivent rester vides en usage normal** : avec PKCS#11 la librairie est auto-détectée, le PIN vient du frontend via `X-Cps-Pin`, et la structure (`Identifiant_Structure`) est lue sur la carte (objets `CPS_ACTIVITY_xx_PS`) — pas en config. `OrganizationId` n'est qu'un fallback pour le mode dégradé sans PKCS#11 ; `Pkcs11Pin` un fallback dev (ne jamais committer).
- `Dmp` — endpoint URLs, `RepositoryUniqueId`, `HomeCommunityId`. `OrganizationSector` (ex: `SA07^1.2.250.1.71.4.2.4`) sert de **sélecteur d'exercice** avec PKCS#11 (voir § Structure d'exercice).
- `ApiKey` — leave empty to disable API key auth in dev.

Avec PKCS#11 (cas normal), aucune valeur de structure n'est requise en config : `/api/cps/card` comme les routes DMP fonctionnent dès que la carte est insérée et le PIN fourni. Le fallback `Cps:OrganizationId` n'entre en jeu que sans PKCS#11 (.p12 dev).

## NuGet caveat

Some packages in `~/.nuget/packages/` are owned by `root` on this machine, blocking restores of newer versions. When adding deps, prefer versions already in the cache (currently: `Microsoft.Extensions.Http 6.0.0`, `Microsoft.Extensions.Options.ConfigurationExtensions 7.0.0`, `Swashbuckle.AspNetCore 6.5.0`). If a restore fails with permission errors, check cache ownership before debugging the package itself.

The `FluentValidation.DependencyInjectionExtensions 11.3.3` reference triggers `NU1603` because only `11.4.0` is available — harmless but noisy. Bump to `11.4.0` if you want a clean build log.

## Logging convention

All Infrastructure services take an injected `ILogger<T>` and log in **French** (`"Carte CPS trouvée"`, `"Erreur PC/SC : ..."`, etc.). Keep new log messages in French for consistency — the audience is French healthcare developers reading the logs.
