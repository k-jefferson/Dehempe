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

The next planned change is to **replace the macOS Swift/CTK path with PKCS#11** via Pkcs11Interop and the already-installed `libcps3_pkcs11_osx.dylib` (under `/usr/local/lib/`). The current `MacOsCtkTokenCertificateProvider` is a transitional fallback — do not invest further in it unless the PKCS#11 route gets vetoed.

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

Layered via the standard ASP.NET Core mechanism:
- `src/Dehempe.API/appsettings.json` — committed defaults.
- `src/Dehempe.API/appsettings.Development.json` — test endpoints, dev cert paths.
- `appsettings.Local.json` (gitignored) or env vars override everything — use this for real CPS credentials.

Key sections:
- `Cps` — `CertificatePath` + `CertificatePassword` (.p12) OR `CertificateThumbprint` (store lookup). `OrganizationId` is the OID of the structure, format `1.2.250.1.71.4.2.2/<FINESS>`.
- `Dmp` — endpoint URLs, `RepositoryUniqueId`, `HomeCommunityId`.
- `ApiKey` — leave empty to disable API key auth in dev.

`appsettings.Development.json` ships with `OrganizationId` set to `<placeholder>`. `/api/cps/card` still works (it doesn't build a VIHF), but **any DMP route** (`/api/patients/{ins}/documents`, etc.) will fail VIHF generation until a real value is set via `appsettings.Local.json` or env vars.

## NuGet caveat

Some packages in `~/.nuget/packages/` are owned by `root` on this machine, blocking restores of newer versions. When adding deps, prefer versions already in the cache (currently: `Microsoft.Extensions.Http 6.0.0`, `Microsoft.Extensions.Options.ConfigurationExtensions 7.0.0`, `Swashbuckle.AspNetCore 6.5.0`). If a restore fails with permission errors, check cache ownership before debugging the package itself.

The `FluentValidation.DependencyInjectionExtensions 11.3.3` reference triggers `NU1603` because only `11.4.0` is available — harmless but noisy. Bump to `11.4.0` if you want a clean build log.

## Logging convention

All Infrastructure services take an injected `ILogger<T>` and log in **French** (`"Carte CPS trouvée"`, `"Erreur PC/SC : ..."`, etc.). Keep new log messages in French for consistency — the audience is French healthcare developers reading the logs.
