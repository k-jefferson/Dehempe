import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DocumentEntry, DocumentListFilter } from './models';

/** Documents DMP (cf. specs/features/F03 & F04). PIN requis. */
@Injectable({ providedIn: 'root' })
export class DocumentsApi {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  /** GET /api/patients/{ins}/documents — liste des métadonnées (ITI-18). */
  list(ins: string, filter: DocumentListFilter = {}): Observable<DocumentEntry[]> {
    let params = new HttpParams();
    if (filter.insOid) params = params.set('insOid', filter.insOid);
    if (filter.createdAfter) params = params.set('createdAfter', filter.createdAfter);
    if (filter.createdBefore) params = params.set('createdBefore', filter.createdBefore);
    if (filter.status) params = params.set('status', filter.status);
    for (const code of filter.classCode ?? []) {
      params = params.append('classCode', code);
    }
    return this.http.get<DocumentEntry[]>(
      `${this.base}/api/patients/${encodeURIComponent(ins)}/documents`,
      { params },
    );
  }

  /**
   * GET …/documents/{uniqueId}/content — contenu binaire (ITI-43).
   * `observe: 'response'` pour lire le Content-Type et un éventuel nom de fichier.
   */
  getContent(
    ins: string,
    uniqueId: string,
    repositoryUniqueId: string,
    homeCommunityId?: string | null,
  ): Observable<HttpResponse<Blob>> {
    let params = new HttpParams().set('repositoryUniqueId', repositoryUniqueId);
    if (homeCommunityId) {
      params = params.set('homeCommunityId', homeCommunityId);
    }
    return this.http.get(
      `${this.base}/api/patients/${encodeURIComponent(ins)}/documents/${encodeURIComponent(uniqueId)}/content`,
      { params, responseType: 'blob', observe: 'response' },
    );
  }
}
