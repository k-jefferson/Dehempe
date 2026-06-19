# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Déhempé is a standalone ASP.NET Core 7 API that reads patient records from the French DMP (Dossier Médical Partagé) via IHE XDS.b SOAP, authenticated through the practitioner's CPS smart card. It is intentionally NOT linked to WEDA — it runs on the practitioner's machine and uses the locally inserted CPS card for identity.

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

1. **Phase publique** (`EnsureLibraryLoaded`, sans PIN) — charge la lib, ouvre la session, trouve le **certificat d'authentification** par `CKA_ID` (dernier octet = `0x20` ; la clé de signature porte `0x10`), mémorise le `CKA_ID` pour appariement ultérieur. Lit aussi les objets `CKO_DATA` publics (`CKA_PRIVATE=false`), dont `CPS_INFO_PS`.
2. **Phase login** (`EnsureLoggedIn`, PIN requis) — déclenchée par `SignWithAuthKey` à la première signature uniquement. Recherche ensuite la **clé privée** correspondante. Les `CKO_PRIVATE_KEY` portent `CKA_PRIVATE=true` et sont **invisibles tant qu'on n'est pas logué** — d'où la séparation.

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

Le **secteur d'activité** vit dans `CPS_ACTIVITY_01_PS` à `CPS_ACTIVITY_15_PS` (4 derniers caractères aussi) — non implémenté à ce jour, le projet utilise `SA07` (libéral) en config. Ces objets sont `CKA_PRIVATE=true` et nécessitent donc le login.

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

## Config

Layered via the standard ASP.NET Core mechanism. **Tous les `src/Dehempe.API/appsettings*.json` sont gitignored** car ils contiennent des données sensibles (OID de structure / FINESS, endpoints DMP par environnement, éventuellement un PIN de dev). Le `.gitignore` ne laisse passer qu'un futur `appsettings.template.json` (sans secret). À l'installation, chaque poste doit reconstituer ses fichiers `appsettings.json`, `appsettings.Development.json` et éventuellement `appsettings.Local.json` localement — soit à la main, soit via env vars (préfixe ASP.NET Core standard : `Dmp__RegistryEndpoint`, `Cps__OrganizationId`, etc.).

Key sections:
- `Cps` — `CertificatePath` + `CertificatePassword` (.p12) OR `CertificateThumbprint` (store lookup). `OrganizationId` is the OID of the structure, format `1.2.250.1.71.4.2.2/<FINESS>`. **`Pkcs11LibraryPath` and `Pkcs11Pin` must stay empty in normal usage** — the library path is auto-detected (see PKCS#11 section), and the PIN comes from the frontend via the `X-Cps-Pin` header at runtime. Renseigner `Pkcs11Pin` est un fallback de dev ; en production cela bypasse l'UI de saisie et ne doit jamais être committé.
- `Dmp` — endpoint URLs, `RepositoryUniqueId`, `HomeCommunityId`.
- `ApiKey` — leave empty to disable API key auth in dev.

`appsettings.Development.json` ships with `OrganizationId` set to `<placeholder>`. `/api/cps/card` still works (it doesn't build a VIHF), but **any DMP route** (`/api/patients/{ins}/documents`, etc.) will fail VIHF generation until a real value is set via `appsettings.Local.json` or env vars.

## NuGet caveat

Some packages in `~/.nuget/packages/` are owned by `root` on this machine, blocking restores of newer versions. When adding deps, prefer versions already in the cache (currently: `Microsoft.Extensions.Http 6.0.0`, `Microsoft.Extensions.Options.ConfigurationExtensions 7.0.0`, `Swashbuckle.AspNetCore 6.5.0`). If a restore fails with permission errors, check cache ownership before debugging the package itself.

The `FluentValidation.DependencyInjectionExtensions 11.3.3` reference triggers `NU1603` because only `11.4.0` is available — harmless but noisy. Bump to `11.4.0` if you want a clean build log.

## Logging convention

All Infrastructure services take an injected `ILogger<T>` and log in **French** (`"Carte CPS trouvée"`, `"Erreur PC/SC : ..."`, etc.). Keep new log messages in French for consistency — the audience is French healthcare developers reading the logs.
