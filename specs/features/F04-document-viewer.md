# F04 — Consultation d'un document (ITI-43)

> **Statut** : 🟡 Spécifié · **Priorité** : Moyenne · **Dépend de** : F03, F05 · **Endpoints** : `GET …/documents/{uniqueId}/content`

## 1. Objectif

Pour le **praticien**, **consulter et/ou télécharger le contenu** d'un document du DMP sélectionné dans la liste (F03).

## 2. Contexte technique — Format CDA R2

Tous les documents du DMP sont au format **HL7 CDA R2** (`application/xml` ou `text/xml`). Deux niveaux coexistent :

| Niveau | Structure CDA | Contenu réel | Affichage |
|--------|--------------|--------------|-----------|
| **N1** | `<nonXMLBody>` | Binaire encodé base64 (PDF dans la grande majorité des cas, mais peut aussi être `text/plain`, `image/*`, etc.) | Extraire le blob binaire, détecter le `mediaType` XDS, afficher directement |
| **N3** | `<structuredBody>` | XML structuré (sections CDA codées, entrées HL7 v3) | Transformer avec la feuille de style XSL ANS |

> **Référence** : `docs/CI-SIS/CI-SIS_CONTENU_VOLET-STRUCTURATION-MINIMALE_V1.16.8.pdf` — profil de structuration minimale du CI-SIS. Lire avant de toucher au parsing CDA.

### 2.1 Détection du niveau

Le niveau CDA est accessible **sans parser le XML** via le champ `formatCode` retourné par F03 (`DocumentEntry.formatCode`). Valeurs ANS :

- `urn:ihe:iti:xds-sd:pdf:2008` ou `urn:ihe:iti:xds-sd:text:2008` → **N1**
- `urn:hl7-org:sdwg:ccda-*` ou tout autre `urn:ihe:pcc:*` structuré → **N3**
- Fallback : parser le XML et tester la présence de `<nonXMLBody>` vs `<structuredBody>`

### 2.2 Extraction du contenu N1 (PDF encapsulé)

```
CDA <nonXMLBody>
  <text mediaType="application/pdf" representation="B64">
    JVBERi0xL...  (base64)
  </text>
</nonXMLBody>
```

- Décoder le base64 → `Blob` JavaScript avec le `mediaType` lu dans l'attribut `@mediaType`.
- Créer un `objectURL` sur ce blob → `<object>` natif du navigateur.
- ⚠️ **`<object [data]>` est un contexte « resource URL »** : Angular rejette une string `blob:` brute
  (erreur `NG0904`, zone vide). Le binding doit recevoir un `SafeResourceUrl` produit par
  `DomSanitizer.bypassSecurityTrustResourceUrl(objectUrl)`. Conserver l'URL brute à part pour
  `URL.revokeObjectURL` à la destruction.
- Le `mimeType` renvoyé par l'API HTTP (`Content-Type`) est `application/xml` (le CDA) — **ne pas** l'utiliser pour déduire le type du contenu encapsulé : toujours lire `@mediaType` dans le XML.

### 2.3 Rendu N3 — transformation XSL ANS

L'ANS publie une feuille de style XSL de référence pour le rendu des CDA R2 en HTML. Elle est à embarquer dans l'application (pas de dépendance réseau au runtime) :

- **Source** : feuille de style officielle ANS (CDA Stylesheet / IHE PCC). À récupérer et versionner sous `src/web/src/assets/cda/cda-style.xsl`.
- **Transformation côté client** : utiliser l'API DOM `XSLTProcessor` (disponible dans tous les navigateurs modernes) :

```typescript
const xsltProcessor = new XSLTProcessor();
xsltProcessor.importStylesheet(xslDocument);         // xslDocument = Document XSL parsé
const resultFragment = xsltProcessor.transformToFragment(cdaDocument, document);
viewerElement.appendChild(resultFragment);
```

- Le résultat est du HTML inline à injecter dans une `<div>` dédiée dans la visionneuse (pas de `<iframe srcdoc>` pour éviter la perte du scope CSS Material 3).
- Sanitiser le HTML produit (Angular `DomSanitizer.bypassSecurityTrustHtml`) — le contenu vient d'un document médical externe.

## 3. Périmètre API

- `GET /api/patients/{ins}/documents/{uniqueId}/content?repositoryUniqueId&homeCommunityId` → **CDA R2 brut** (`application/xml`).
- `uniqueId` **URL-encodé** ; `repositoryUniqueId` obligatoire.
- Auth : **PIN requis** (F05). Récupération en `responseType: 'text'` (XML string) côté Angular.

> Note : l'API renvoie le XML brut tel que retourné par le DMP (ITI-43). Tout le parsing et la transformation CDA ont lieu **côté frontend**.

## 4. User stories & critères d'acceptation

### US-04.1 — Visionner un document CDA N1 (PDF encapsulé)
**En tant que** praticien, **je veux** afficher le PDF contenu dans un document CDA N1 **afin de** le lire sans quitter l'app.
- [ ] Le XML CDA est parsé côté frontend.
- [ ] Le contenu `<nonXMLBody>` est extrait (base64 décodé) avec son `@mediaType`.
- [ ] Si `mediaType = application/pdf` → visionneuse PDF intégrée (`<object>` avec object URL).
- [ ] Si `mediaType` est un autre type affichable (`text/plain`, `image/*`) → rendu adapté.
- [ ] Object URL révoqué à la fermeture (pas de fuite mémoire).

### US-04.2 — Visionner un document CDA N3 (contenu structuré)
**En tant que** praticien, **je veux** voir le rendu lisible d'un document CDA N3 **afin d'en consulter le contenu**.
- [ ] La feuille de style XSL ANS est chargée depuis `assets/cda/cda-style.xsl`.
- [ ] La transformation `XSLTProcessor` produit du HTML.
- [ ] Le HTML est injecté dans la zone visionneuse (sanitisé via `DomSanitizer`).
- [ ] En cas d'échec de la transformation → message d'erreur + proposition de téléchargement.

### US-04.3 — Télécharger
- [ ] Bouton « Télécharger » disponible dans tous les cas.
- [ ] Pour N1 : télécharge le contenu encapsulé (le PDF, pas le CDA).
- [ ] Pour N3 : télécharge le CDA XML brut avec extension `.xml`.
- [ ] Nom de fichier : titre du document (F03) + extension dérivée du type.

### US-04.4 — Type non affichable (N1 non-PDF)
- [ ] Si `@mediaType` n'est pas affichable dans le navigateur → aperçu indisponible + téléchargement automatiquement proposé avec message clair.

### US-04.5 — Erreurs
- [ ] `404` → « Document introuvable. »
- [ ] `401` → PIN (F05).
- [ ] Erreur de parsing CDA → « Le document ne peut pas être affiché. » + téléchargement.

## 5. Règles métier & validations

- Toujours partir des identifiants fournis par F03 (`uniqueId`, `repositoryUniqueId`, `homeCommunityId`, `formatCode`, `mimeType`).
- Encoder `uniqueId` (`encodeURIComponent`).
- **Ne jamais** utiliser le `Content-Type` HTTP pour déduire le type du contenu médical — toujours lire le XML CDA.
- **Révoquer** les object URLs créés (`URL.revokeObjectURL`) à la fermeture.
- Le contenu n'est **pas** persisté sur disque par l'app (téléchargement = action explicite utilisateur).
- La feuille XSL est versionnée dans le repo et chargée une seule fois (cache en mémoire, pas de rechargement par document).

## 6. UI / composants (Material 3)

- En-tête avec titre du document + bouton retour (vers F03) + bouton « Télécharger ».
- **Zone visionneuse** :
  - N1 PDF : `<object type="application/pdf" [data]="objectUrl">` avec fallback `<iframe>`.
  - N3 : `<div class="cda-viewer">` recevant le HTML transformé (scroll interne).
  - Non supporté : carte « Aperçu indisponible » + bouton de téléchargement.
- Indicateur de niveau CDA (badge ou chip discret : « Document PDF » / « Document structuré ») pour information praticien.

## 7. États & erreurs

- **Chargement** : « Récupération du document… » (spinner).
- **Transformation** : « Mise en forme du document… » (pour N3, si transformation longue).
- **Erreur** : 404 / 401 / 502 / parsing CDA (cf. ux-writing).
- **Non supporté** : aperçu indisponible → téléchargement.

## 8. Definition of Done

- [ ] US-04.1 → 04.5 implémentées.
- [ ] Feuille XSL ANS versionnée sous `src/web/src/assets/cda/cda-style.xsl`.
- [ ] Object URL révoqué à la fermeture du viewer.
- [ ] États loading / erreur couverts.
- [ ] Tokens M3 uniquement.
- [ ] `npm run build` OK (0 erreur TypeScript).
