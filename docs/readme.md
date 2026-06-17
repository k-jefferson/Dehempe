# Déhempé — Guide de démarrage

API d'accès au DMP (Dossier Médical Partagé) via IHE XDS.b.  
L'identité du praticien est lue automatiquement depuis la carte CPS branchée en PC/SC.

---

## Prérequis

| Élément | Version minimale |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 7.0 |
| Middleware CPS | installé et fonctionnel |
| Lecteur de carte PC/SC | branché avec la carte CPS insérée |

Vérifier que le SDK est bien installé :

```bash
dotnet --version
# doit afficher 7.x.x
```

---

## Configuration

Copier le fichier de configuration de développement et l'adapter :

```bash
cp src/Dehempe.API/appsettings.Development.json src/Dehempe.API/appsettings.Local.json
```

Ouvrir `appsettings.Local.json` et renseigner au minimum :

```json
{
  "Cps": {
    "CertificatePath": "chemin/vers/cps.p12",
    "CertificatePassword": "mot-de-passe",
    "OrganizationId": "1.2.250.1.71.4.2.2/XXXXXXXXX"
  },
  "Dmp": {
    "RepositoryUniqueId": "1.2.250.1.213.1.2.12"
  }
}
```

> **CertificatePath** : chemin vers le fichier `.p12` exporté depuis le magasin système,  
> **ou** laisser vide et renseigner `CertificateThumbprint` pour lire directement depuis le magasin.  
> **OrganizationId** : OID de ta structure au format `1.2.250.1.71.4.2.2/<FINESS>`.

### Trouver le thumbprint d'un certificat dans le magasin système

**macOS**

```bash
security find-certificate -a -p | openssl x509 -noout -fingerprint -sha1
```

**Windows (PowerShell)**

```powershell
Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -like "*CPS*" } | Select-Object Thumbprint, Subject
```

---

## Lancer l'API

```bash
cd src/Dehempe.API
dotnet run
```

L'API démarre sur `https://localhost:7xxx` et `http://localhost:5xxx`  
(les ports exacts sont affichés dans la console au démarrage).

### Spécifier un environnement

```bash
# Développement (active Swagger, logs détaillés)
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Production
ASPNETCORE_ENVIRONMENT=Production dotnet run
```

### Utiliser un fichier de config personnalisé

```bash
dotnet run --launch-profile http
```

Ou passer directement une valeur de configuration :

```bash
dotnet run -- --Cps:CertificatePath=/chemin/cps.p12
```

---

## Interface Swagger

En environnement `Development`, l'interface Swagger est disponible à :

```
http://localhost:<port>/swagger
```

Elle liste toutes les routes disponibles et permet de les tester directement.

---

## Routes principales

### Lire la carte CPS branchée

```bash
curl -s http://localhost:<port>/api/cps/card | jq
```

Retourne les données du praticien extraites du certificat de la carte (RPPS, nom, rôle, structure, validité du certificat).

### Lister les documents DMP d'un patient

```bash
curl -s "http://localhost:<port>/api/patients/<INS>/documents" | jq
```

Remplacer `<INS>` par le NIR du patient (15 chiffres).

Filtres disponibles en query string :

```bash
curl -s "http://localhost:<port>/api/patients/269054938412345/documents?status=APPROVED&createdAfter=2023-01-01" | jq
```

### Télécharger le contenu d'un document

```bash
curl -s "http://localhost:<port>/api/patients/<INS>/documents/<uniqueId>/content?repositoryUniqueId=<repoOid>" \
     --output document.pdf
```

---

## Protéger l'API avec une clé

Pour activer la protection par clé API, renseigner `ApiKey:ApiKey` dans la configuration :

```json
{
  "ApiKey": {
    "ApiKey": "ma-cle-secrete"
  }
}
```

Puis passer la clé dans chaque requête :

```bash
curl -H "X-Api-Key: ma-cle-secrete" http://localhost:<port>/api/cps/card | jq
```

Si `ApiKey` est vide ou absent, toutes les requêtes sont acceptées (comportement par défaut en développement local).

---

## Publier (build de production)

```bash
dotnet publish src/Dehempe.API -c Release -o ./publish
cd publish
dotnet Dehempe.API.dll
```
