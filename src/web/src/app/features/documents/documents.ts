import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  input,
  signal,
} from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { catchError, map, of, startWith, switchMap } from 'rxjs';
import { Router } from '@angular/router';
import { DocumentsApi } from '../../core/api/documents-api';
import { DocumentEntry } from '../../core/api/models';
import { PatientsDataset } from '../../core/patients/patients-dataset';

/** OID de l'INS — choisi par longueur (cf. specs/features/F02 & F07). */
const INS_OID_NIR = '1.2.250.1.213.1.4.8';
const INS_OID_NIA = '1.2.250.1.213.1.4.9';

/** Ligne d'affichage dérivée d'un `DocumentEntry` (calculée une fois). */
interface DocRow {
  uniqueId: string;
  title: string;
  hasTitle: boolean;
  type: string;
  typeTooltip: string;
  icon: string;
  creationTime: string | null;
  sortDate: number;
  author: string;
  /** Valeur brute (HL7 XCN/XON) en infobulle, seulement si elle diffère du nom affiché. */
  authorTooltip: string;
}

/** État de l'écran : la requête DMP est en cours, aboutie, ou en erreur. */
type State =
  | { kind: 'loading' }
  | { kind: 'loaded'; documents: DocumentEntry[]; dmpRequest: string | null; dmpResponse: string | null }
  | { kind: 'error'; status: number; message: string };

/**
 * Liste des documents du DMP d'un patient (cf. `specs/features/F03-document-list.md`),
 * ouverte par la sélection d'un patient dans le sidenav (`specs/features/F07-...`).
 *
 * L'INS provient du paramètre de route `:ins` (`withComponentInputBinding`). Le nom/prénom de
 * l'en-tête sont résolus **localement** dans le jeu d'essai F06 (pas via le DMP). Le PIN (F05) est
 * géré de façon transparente par `pinInterceptor`.
 */
@Component({
  selector: 'app-documents',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [provideNativeDateAdapter()],
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatSortModule,
    MatTableModule,
    MatTooltipModule,
  ],
  templateUrl: './documents.html',
  styleUrl: './documents.scss',
})
export class Documents {
  private readonly api = inject(DocumentsApi);
  private readonly dataset = inject(PatientsDataset);
  private readonly router = inject(Router);

  /** INS du patient, lié au paramètre de route `:ins`. */
  readonly ins = input.required<string>();

  /** Nom affiché du patient (jeu d'essai F06, lookup par INS) — `null` si l'INS n'y figure pas. */
  readonly patientName = computed<string | null>(() => {
    const ins = this.ins();
    const p = this.dataset
      .patients()
      .find((x) => x.matriculeInsNir != null && String(x.matriculeInsNir) === ins);
    if (!p) return null;
    const nom = p.nomUtilise ?? p.nomDeNaissance ?? '';
    const prenom = p.prenomUtilise ?? p.prenomDeNaissance ?? '';
    return `${nom} ${prenom}`.trim() || null;
  });

  /** Fenêtre temporelle (défaut J−30 → aujourd'hui, cf. F03). */
  readonly from = signal<Date>(daysAgo(30));
  readonly to = signal<Date>(new Date());

  /** Compteur d'actualisation : permet de relancer la même requête (après PIN, erreur transitoire). */
  private readonly reloadNonce = signal(0);

  private readonly request = computed(() => ({
    ins: this.ins(),
    insOid: this.ins().length === 15 ? INS_OID_NIR : INS_OID_NIA,
    createdAfter: startOfDay(this.from()).toISOString(),
    createdBefore: endOfDay(this.to()).toISOString(),
    nonce: this.reloadNonce(),
  }));

  private readonly state = toSignal(
    toObservable(this.request).pipe(
      switchMap((req) =>
        this.api
          .list(req.ins, {
            insOid: req.insOid,
            createdAfter: req.createdAfter,
            createdBefore: req.createdBefore,
          })
          .pipe(
            map(
              (list): State => ({
                kind: 'loaded',
                documents: list.documents,
                dmpRequest: list.dmpRequest,
                dmpResponse: list.dmpResponse,
              }),
            ),
            startWith<State>({ kind: 'loading' }),
            catchError((err: unknown) => of<State>(toErrorState(err))),
          ),
      ),
    ),
    { initialValue: { kind: 'loading' } as State },
  );

  readonly loading = computed(() => this.state().kind === 'loading');
  readonly hardError = computed(() => {
    const s = this.state();
    return s.kind === 'error' && s.status !== 404 ? s : null;
  });
  readonly notFound = computed(() => {
    const s = this.state();
    return s.kind === 'error' && s.status === 404;
  });
  private readonly loaded = computed(() => {
    const s = this.state();
    return s.kind === 'loaded' ? s : null;
  });

  readonly rows = computed<DocRow[]>(() => (this.loaded()?.documents ?? []).map(toRow));
  readonly total = computed(() => this.rows().length);

  /** Diagnostic affiché quand la liste est vide (XML SOAP brut renvoyé par l'API). */
  readonly diagnostic = computed(() => {
    const s = this.loaded();
    if (!s || s.documents.length > 0) return null;
    return { request: s.dmpRequest, response: s.dmpResponse };
  });

  // Tri + pagination, pilotés par signaux (jeux de petite taille → tri/pagination côté client).
  readonly displayedColumns = ['type', 'title', 'creationTime', 'author'];
  readonly sort = signal<Sort>({ active: 'creationTime', direction: 'desc' });
  readonly pageIndex = signal(0);
  readonly pageSize = signal(10);

  private readonly sortedRows = computed(() => sortRows(this.rows(), this.sort()));
  readonly pagedRows = computed(() => {
    const start = this.pageIndex() * this.pageSize();
    return this.sortedRows().slice(start, start + this.pageSize());
  });

  constructor() {
    // Revenir à la 1re page quand la requête change (autre patient ou fenêtre temporelle).
    effect(() => {
      this.request();
      this.pageIndex.set(0);
    });
  }

  onSort(s: Sort): void {
    this.sort.set(s);
    this.pageIndex.set(0); // un nouveau tri ramène à la première page (comportement attendu d'un tableau).
  }
  onPage(e: PageEvent): void {
    this.pageIndex.set(e.pageIndex);
    this.pageSize.set(e.pageSize);
  }
  onFrom(d: Date | null): void {
    if (d) this.from.set(d);
  }
  onTo(d: Date | null): void {
    if (d) this.to.set(d);
  }
  resetFilters(): void {
    this.from.set(daysAgo(30));
    this.to.set(new Date());
  }
  reload(): void {
    this.reloadNonce.update((n) => n + 1);
  }

  openDocument(row: DocRow): void {
    const ins = this.ins();
    const doc = this.loaded()?.documents.find((d) => d.uniqueId === row.uniqueId);
    if (!doc) return;
    this.router.navigate(
      ['/patient', ins, 'documents', encodeURIComponent(doc.uniqueId)],
      {
        queryParams: {
          repositoryUniqueId: doc.repositoryUniqueId,
          ...(doc.homeCommunityId ? { homeCommunityId: doc.homeCommunityId } : {}),
          title: doc.title ?? row.title,
          ...(doc.formatCode ? { formatCode: doc.formatCode } : {}),
        },
      },
    );
  }
}

function daysAgo(n: number): Date {
  const d = new Date();
  d.setDate(d.getDate() - n);
  return d;
}
function startOfDay(d: Date): Date {
  const x = new Date(d);
  x.setHours(0, 0, 0, 0);
  return x;
}
function endOfDay(d: Date): Date {
  const x = new Date(d);
  x.setHours(23, 59, 59, 999);
  return x;
}

function iconForMime(mime: string | null): string {
  if (!mime) return 'description';
  if (mime.includes('pdf')) return 'picture_as_pdf';
  if (mime.startsWith('image/')) return 'image';
  if (mime.includes('xml') || mime.includes('html')) return 'code';
  if (mime.startsWith('text/')) return 'article';
  return 'description';
}

function toRow(doc: DocumentEntry): DocRow {
  const title = doc.title?.trim() ?? '';
  const rawAuthor = doc.authorPerson ?? doc.authorInstitution ?? '';
  const author = formatPersonName(doc.authorPerson) ?? formatInstitution(doc.authorInstitution) ?? '—';
  return {
    uniqueId: doc.uniqueId,
    title: title || 'Document sans titre',
    hasTitle: title.length > 0,
    type: doc.classCode ?? doc.typeCode ?? '—',
    typeTooltip:
      `classCode : ${doc.classCode ?? '—'}\n` +
      `typeCode : ${doc.typeCode ?? '—'}\n` +
      `formatCode : ${doc.formatCode ?? '—'}\n` +
      `mimeType : ${doc.mimeType ?? '—'}`,
    icon: iconForMime(doc.mimeType),
    creationTime: doc.creationTime,
    sortDate: doc.creationTime ? Date.parse(doc.creationTime) : 0,
    author,
    authorTooltip: rawAuthor && rawAuthor !== author ? rawAuthor : '',
  };
}

/**
 * Nom lisible d'une **personne** depuis un champ HL7 **XCN** renvoyé par le DMP
 * (`id^nom^prénom^…^&autorité&ISO^type`). Ex. `899700780259^MEDECIN^VIRGINIE^…` → « VIRGINIE MEDECIN ».
 * À défaut de composantes nom/prénom, on retombe sur l'identifiant (1re composante).
 */
function formatPersonName(xcn: string | null): string | null {
  if (!xcn) return null;
  const parts = xcn.split('^');
  const family = (parts[1] ?? '').trim();
  const given = (parts[2] ?? '').trim();
  const name = [given, family].filter(Boolean).join(' ');
  return name || (parts[0] ?? '').trim() || null;
}

/**
 * Nom lisible d'une **institution** depuis un champ HL7 **XON** (`nom^…`) : 1re composante.
 */
function formatInstitution(xon: string | null): string | null {
  if (!xon) return null;
  return (xon.split('^')[0] ?? '').trim() || null;
}

function sortRows(rows: DocRow[], sort: Sort): DocRow[] {
  if (!sort.active || sort.direction === '') return rows;
  const dir = sort.direction === 'asc' ? 1 : -1;
  return [...rows].sort((a, b) => {
    switch (sort.active) {
      case 'creationTime':
        return (a.sortDate - b.sortDate) * dir;
      case 'title':
        return a.title.localeCompare(b.title, 'fr') * dir;
      case 'type':
        return a.type.localeCompare(b.type, 'fr') * dir;
      case 'author':
        return a.author.localeCompare(b.author, 'fr') * dir;
      default:
        return 0;
    }
  });
}

function toErrorState(err: unknown): State {
  if (err instanceof HttpErrorResponse) {
    switch (err.status) {
      case 404:
        return { kind: 'error', status: 404, message: 'Aucun DMP trouvé pour ce patient (ou patient introuvable).' };
      case 502:
        return { kind: 'error', status: 502, message: 'Le DMP a renvoyé une erreur.' };
      case 401:
        return { kind: 'error', status: 401, message: "Authentification requise (PIN refusé ou clé d'API invalide)." };
      case 400:
        return { kind: 'error', status: 400, message: 'INS invalide.' };
    }
  }
  return { kind: 'error', status: 0, message: 'Une erreur inattendue est survenue. Réessayez.' };
}
