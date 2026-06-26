import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

/** Résultat de l'extraction d'un CDA N1. */
export interface CdaN1Content {
  blob: Blob;
  mediaType: string;
}

/**
 * Service de parsing et rendu des documents CDA R2 (DMP / CI-SIS).
 *
 * Deux niveaux :
 *  - N1 : `<nonXMLBody>` → contenu binaire base64 (PDF ou autre), extrait et converti en Blob.
 *  - N3 : `<structuredBody>` → transformé en HTML via la feuille XSL ANS (XSLTProcessor).
 *
 * La feuille XSL est chargée une seule fois (cache en mémoire) depuis `assets/cda/cda-style.xsl`.
 */
@Injectable({ providedIn: 'root' })
export class CdaService {
  private readonly http = inject(HttpClient);
  private xslCache: Document | null = null;

  /** Détecte le niveau CDA à partir du XML brut. */
  detectLevel(xmlText: string): 'N1' | 'N3' | 'unknown' {
    const doc = parseCdaXml(xmlText);
    if (!doc) return 'unknown';
    const ns = 'urn:hl7-org:v3';
    if (doc.getElementsByTagNameNS(ns, 'nonXMLBody').length > 0) return 'N1';
    if (doc.getElementsByTagNameNS(ns, 'structuredBody').length > 0) return 'N3';
    return 'unknown';
  }

  /**
   * Extrait le contenu binaire d'un CDA N1 (`<nonXMLBody><text mediaType="…">base64</text>`).
   * Retourne `null` si le XML est invalide ou ne contient pas de nonXMLBody.
   */
  extractN1Content(xmlText: string): CdaN1Content | null {
    const doc = parseCdaXml(xmlText);
    if (!doc) return null;
    const ns = 'urn:hl7-org:v3';
    const textEl = doc.getElementsByTagNameNS(ns, 'nonXMLBody')[0]
      ?.getElementsByTagNameNS(ns, 'text')[0];
    if (!textEl) return null;
    const mediaType = textEl.getAttribute('mediaType') ?? 'application/octet-stream';
    const b64 = textEl.textContent?.trim() ?? '';
    if (!b64) return null;
    return { blob: base64ToBlob(b64, mediaType), mediaType };
  }

  /**
   * Transforme un CDA N3 en HTML via `XSLTProcessor` et la feuille de style ANS.
   * Retourne une chaîne HTML prête à être injectée dans le DOM (à sanitiser via DomSanitizer).
   */
  async renderN3(xmlText: string): Promise<string> {
    const [cdaDoc, xslDoc] = await Promise.all([
      Promise.resolve(parseCdaXml(xmlText)),
      this.loadXsl(),
    ]);
    if (!cdaDoc) throw new Error('XML CDA invalide');
    const processor = new XSLTProcessor();
    processor.importStylesheet(xslDoc);
    const fragment = processor.transformToFragment(cdaDoc, document);
    if (!fragment) throw new Error('Échec de la transformation XSL');
    const wrapper = document.createElement('div');
    wrapper.appendChild(fragment);
    return wrapper.innerHTML;
  }

  private async loadXsl(): Promise<Document> {
    if (this.xslCache) return this.xslCache;
    const text = await firstValueFrom(
      this.http.get('/cda/cda-style.xsl', { responseType: 'text' }),
    );
    const parser = new DOMParser();
    const doc = parser.parseFromString(text, 'application/xml');
    const parseError = doc.querySelector('parsererror');
    if (parseError) throw new Error(`Erreur de parsing XSL : ${parseError.textContent}`);
    this.xslCache = doc;
    return doc;
  }
}

function parseCdaXml(xmlText: string): Document | null {
  try {
    const parser = new DOMParser();
    const doc = parser.parseFromString(xmlText, 'application/xml');
    if (doc.querySelector('parsererror')) return null;
    return doc;
  } catch {
    return null;
  }
}

function base64ToBlob(b64: string, mediaType: string): Blob {
  const clean = b64.replace(/\s+/g, '');
  const binary = atob(clean);
  const bytes = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i);
  }
  return new Blob([bytes], { type: mediaType });
}
