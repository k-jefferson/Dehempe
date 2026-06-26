import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { DomSanitizer, SafeHtml, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { HttpErrorResponse } from '@angular/common/http';
import { DocumentsApi } from '../../core/api/documents-api';
import { CdaService, CdaN1Content } from '../../core/cda/cda.service';
import { ApiProblem } from '../../core/api/models/api-problem.model';

type CdaLevel = 'N1' | 'N3' | 'unknown';

/** Détails SOAP portés dans le corps d'une erreur 502 DMP. */
export interface DmpErrorDetails {
  soapResponse: string | null;
  soapRequest: string | null;
}

type ViewerState =
  | { kind: 'loading'; label: string }
  | { kind: 'pdf'; safeUrl: SafeResourceUrl; title: string }
  | { kind: 'n1-other'; blob: Blob; mediaType: string; title: string }
  | { kind: 'n3'; html: SafeHtml; title: string }
  | { kind: 'unsupported'; title: string; cdaXml: string }
  | { kind: 'error'; message: string; title: string; dmpDetails: DmpErrorDetails | null };

/**
 * Visionneuse de document CDA R2 (F04).
 *
 * Route : `patient/:ins/documents/:uniqueId`
 * Query params : `repositoryUniqueId` (obligatoire), `homeCommunityId`, `title`, `formatCode`.
 *
 * - CDA N1 (nonXMLBody) : extrait le blob encodé en base64, crée un object URL, affiche
 *   le PDF natif du navigateur ou signale un aperçu indisponible pour les autres types.
 * - CDA N3 (structuredBody) : transforme via XSLTProcessor + feuille XSL ANS, injecte le HTML.
 * - Téléchargement : PDF (N1) ou XML brut (N3) selon le niveau détecté.
 */
@Component({
  selector: 'app-document-viewer',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatButtonModule,
    MatCardModule,
    MatExpansionModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
  ],
  templateUrl: './document-viewer.html',
  styleUrl: './document-viewer.scss',
})
export class DocumentViewer implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(DocumentsApi);
  private readonly cda = inject(CdaService);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly destroyRef = inject(DestroyRef);

  readonly state = signal<ViewerState>({ kind: 'loading', label: 'Récupération du document…' });

  /** Blob de téléchargement : contenu encapsulé (N1) ou XML brut (N3). */
  private downloadBlob: Blob | null = null;
  /** Nom de fichier dérivé du titre + niveau. */
  private downloadFilename = 'document';
  /** Object URL à révoquer à la destruction du composant. */
  private objectUrl: string | null = null;
  /** XML CDA brut conservé pour le téléchargement N3 et le diagnostic. */
  private rawXml: string | null = null;
  /** Niveau CDA détecté (pour le badge d'information). */
  readonly cdaLevel = signal<CdaLevel | null>(null);

  ngOnInit(): void {
    const params = this.route.snapshot.paramMap;
    const query = this.route.snapshot.queryParamMap;
    const ins = params.get('ins') ?? '';
    const uniqueId = params.get('uniqueId') ?? '';
    const repositoryUniqueId = query.get('repositoryUniqueId') ?? '';
    const homeCommunityId = query.get('homeCommunityId') ?? null;
    const title = query.get('title') ?? 'Document sans titre';

    this.downloadFilename = sanitizeFilename(title);

    // Révoquer l'object URL à la destruction du composant (pas de fuite mémoire).
    this.destroyRef.onDestroy(() => {
      if (this.objectUrl) URL.revokeObjectURL(this.objectUrl);
    });

    this.loadDocument(ins, uniqueId, repositoryUniqueId, homeCommunityId, title);
  }

  private async loadDocument(
    ins: string,
    uniqueId: string,
    repositoryUniqueId: string,
    homeCommunityId: string | null,
    title: string,
  ): Promise<void> {
    try {
      const xmlText = await this.api.getContent(ins, uniqueId, repositoryUniqueId, homeCommunityId);
      this.rawXml = xmlText;

      const level = this.cda.detectLevel(xmlText);
      this.cdaLevel.set(level);

      if (level === 'N1') {
        await this.handleN1(xmlText, title);
      } else if (level === 'N3') {
        await this.handleN3(xmlText, title);
      } else {
        // Niveau inconnu : proposer le téléchargement du XML brut.
        this.downloadBlob = new Blob([xmlText], { type: 'application/xml' });
        this.downloadFilename += '.xml';
        this.state.set({ kind: 'unsupported', title, cdaXml: xmlText });
      }
    } catch (err: unknown) {
      const { message, dmpDetails } = await parseHttpError(err);
      this.state.set({ kind: 'error', message, title, dmpDetails });
    }
  }

  /** Signal exposant les détails SOAP pour le template (facilite l'accès sans cast). */
  readonly errorDmpDetails = () => {
    const s = this.state();
    return s.kind === 'error' ? s.dmpDetails : null;
  };

  private async handleN1(xmlText: string, title: string): Promise<void> {
    const content: CdaN1Content | null = this.cda.extractN1Content(xmlText);
    if (!content) {
      this.state.set({ kind: 'error', message: 'Le contenu du document ne peut pas être extrait.', title, dmpDetails: null });
      return;
    }
    const { blob, mediaType } = content;
    const ext = mimeToExtension(mediaType);
    this.downloadBlob = blob;
    this.downloadFilename += ext;

    if (mediaType === 'application/pdf' || mediaType.startsWith('image/') || mediaType.startsWith('text/')) {
      const objectUrl = URL.createObjectURL(blob);
      this.objectUrl = objectUrl;
      if (mediaType === 'application/pdf') {
        // `<object data>` est un contexte « resource URL » : Angular exige un SafeResourceUrl
        // (une string blob: brute déclenche NG0904). L'URL brute reste dans this.objectUrl
        // pour la révocation à la destruction.
        this.state.set({
          kind: 'pdf',
          safeUrl: this.sanitizer.bypassSecurityTrustResourceUrl(objectUrl),
          title,
        });
      } else {
        this.state.set({ kind: 'n1-other', blob, mediaType, title });
      }
    } else {
      this.state.set({ kind: 'unsupported', title, cdaXml: xmlText });
    }
  }

  private async handleN3(xmlText: string, title: string): Promise<void> {
    this.state.set({ kind: 'loading', label: 'Mise en forme du document…' });
    try {
      const htmlStr = await this.cda.renderN3(xmlText);
      this.downloadBlob = new Blob([xmlText], { type: 'application/xml' });
      this.downloadFilename += '.xml';
      this.state.set({
        kind: 'n3',
        html: this.sanitizer.bypassSecurityTrustHtml(htmlStr),
        title,
      });
    } catch {
      // Transformation XSL échouée → proposer le téléchargement.
      this.downloadBlob = new Blob([xmlText], { type: 'application/xml' });
      this.downloadFilename += '.xml';
      this.state.set({ kind: 'unsupported', title, cdaXml: xmlText });
    }
  }

  download(): void {
    let blob = this.downloadBlob;
    if (!blob && this.rawXml) {
      blob = new Blob([this.rawXml], { type: 'application/xml' });
    }
    if (!blob) return;
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = this.downloadFilename;
    a.click();
    URL.revokeObjectURL(url);
  }

  back(): void {
    // Retour vers la liste des documents du même patient.
    const ins = this.route.snapshot.paramMap.get('ins') ?? '';
    this.router.navigate(['/patient', ins, 'documents']);
  }
}

function sanitizeFilename(name: string): string {
  return name.replace(/[^a-zA-Z0-9À-ÿ\-_ ]/g, '_').slice(0, 80).trim() || 'document';
}

function mimeToExtension(mime: string): string {
  if (mime === 'application/pdf') return '.pdf';
  if (mime.startsWith('image/png')) return '.png';
  if (mime.startsWith('image/jpeg')) return '.jpg';
  if (mime.startsWith('image/')) return '.img';
  if (mime.startsWith('text/plain')) return '.txt';
  return '';
}

async function parseHttpError(err: unknown): Promise<{ message: string; dmpDetails: DmpErrorDetails | null }> {
  if (!(err instanceof HttpErrorResponse)) {
    return { message: 'Une erreur inattendue est survenue. Réessayez.', dmpDetails: null };
  }
  switch (err.status) {
    case 404: return { message: 'Document introuvable.', dmpDetails: null };
    case 401: return { message: 'Authentification requise (PIN).', dmpDetails: null };
    case 502: {
      const problem = await readProblemBody(err);
      return {
        message: 'Le DMP a renvoyé une erreur.',
        dmpDetails: {
          soapResponse: problem?.response ?? null,
          soapRequest: problem?.request ?? null,
        },
      };
    }
    default: return { message: 'Une erreur inattendue est survenue. Réessayez.', dmpDetails: null };
  }
}

async function readProblemBody(err: HttpErrorResponse): Promise<ApiProblem | null> {
  try {
    if (err.error instanceof Blob) {
      const text = await err.error.text();
      return JSON.parse(text) as ApiProblem;
    }
    if (typeof err.error === 'string') {
      return JSON.parse(err.error) as ApiProblem;
    }
    if (typeof err.error === 'object') {
      return err.error as ApiProblem;
    }
  } catch { /* parsing échoué → pas de détails */ }
  return null;
}
