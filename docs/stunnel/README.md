# Tunnel mTLS local pour macOS — stunnel + PKCS#11

## Pourquoi

.NET sur macOS ne sait pas attacher un certificat client PKCS#11 au handshake
mTLS (la stack Apple exige l'export de la clé privée, ce qu'un token de carte
refuse par design). `stunnel` contourne ça en terminant le mTLS localement
côté carte, et expose un endpoint HTTP loopback au process .NET.

```
.NET App  ──HTTP plain──►  stunnel  ──HTTPS + mTLS──►  DMP
          (Host=DMP)        + engine_pkcs11
                            + libcps3
```

## Prérequis

```bash
brew install stunnel libp11
```

- `stunnel` : le proxy TLS
- `libp11` : fournit l'engine OpenSSL `pkcs11` que stunnel utilise pour
  parler à `libcps3_pkcs11_osx.dylib`

## Vérifier l'engine

OpenSSL 3.6+ (Homebrew) cherche les engines dans son Cellar par défaut alors que
`libp11` les installe sous `/opt/homebrew/lib/engines-3`. Exporte `OPENSSL_ENGINES`
pour que stunnel les trouve :

```bash
export OPENSSL_ENGINES=/opt/homebrew/lib/engines-3
openssl engine -t pkcs11   # → "(pkcs11) pkcs11 engine [ available ]"
```

## Lancer

Depuis la racine du repo, **avec** `OPENSSL_ENGINES` exporté :

```bash
OPENSSL_ENGINES=/opt/homebrew/lib/engines-3 stunnel docs/stunnel/dehempe-cps.conf
```

Vérifier qu'il écoute :

```bash
lsof -nP -iTCP:5443 | grep LISTEN
```

Logs : `/tmp/dehempe-stunnel.log` (`tail -f` utile en cas de souci).

## Arrêter

```bash
kill "$(cat /tmp/dehempe-stunnel.pid)"
```

## Configurer Déhempé pour utiliser le tunnel

Le profil `Development` (committé) est déjà configuré :

```json
"Dmp": {
  "RegistryEndpoint":  "https://lps.dev1.dmp.gouv.fr/si-dmp-server/v2/services",
  "RepositoryEndpoint":"https://lps.dev1.dmp.gouv.fr/si-dmp-server/v2/services",
  "GdpEndpoint":       "https://lps.dev1.dmp.gouv.fr/si-dmp-server/v2/services",
  "TunnelEndpoint":    "http://127.0.0.1:5443"
}
```

- Les `*Endpoint` restent les URLs DMP **logiques** (préservent le `Host` header
  envoyé au serveur DMP).
- `TunnelEndpoint` redirige le TCP/TLS vers le tunnel local. Vide = mTLS direct
  (Windows / Linux).

`DmpTunnelHandler` réécrit l'URL au moment de l'envoi : `Scheme/Host/Port` ←
tunnel, `Host:` header ← URL d'origine.

## Désactiver le tunnel (prod Windows / Linux)

Mettre `Dmp:TunnelEndpoint` à `""` dans `appsettings.Local.json` ou via env :

```bash
export Dmp__TunnelEndpoint=""
```

.NET attachera alors directement le cert CPS au handshake mTLS via PKCS#11 (ou
.p12 / store selon la conf).

## Sécurité

Le PIN apparaît en clair dans `dehempe-cps.conf`. C'est un fichier local de dev,
non commité dans `appsettings.Local.json` (qui est gitignored). Pour la prod :
- Stocker `dehempe-cps.conf` hors du repo, perms `chmod 600`
- Ou passer par un secret manager (`/etc/stunnel/`-style, owner root)

## ⚠️ Limitation connue : conflit d'accès à la carte avec la signature VIHF

stunnel utilise PKCS#11 (libcps3) pour signer le ClientCertVerify du handshake
mTLS. Si en parallèle l'API .NET signe la VIHF via CTK
(`SecKeyCreateSignature`), Apple `tokend` accède à la même carte et **invalide
l'état PIN-verified** de la session PKCS#11 de stunnel (PIN-verified est card-
global, partagé entre les stacks). Symptôme côté stunnel :
`error:42000101:PKCS#11 module::User not logged in`.

**Validation isolée** (curl direct, sans .NET CTK actif) : le tunnel marche
de bout en bout, le DMP renvoie un SOAP Fault métier propre.

**Voies de sortie** :
- **Dev** : demander à l'ANS un `.p12` de test pour la signature VIHF ; le mTLS
  reste via PKCS#11/stunnel, la VIHF est signée par le .p12 logiciel — aucun
  accès carte côté .NET.
- **Prod (Windows)** : pas concerné. CNG sérialise les accès et le middleware
  CPS expose le cert avec une clé "utilisable" pour .NET TLS sans tunnel.
- **Long terme** : `p11-kit` peut proxifier libcps3 entre plusieurs processus
  pour sérialiser les sessions. À évaluer si on veut un dev macOS complet.
