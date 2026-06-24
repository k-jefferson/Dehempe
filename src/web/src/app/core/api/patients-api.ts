import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DmpExistence, InsOid } from './models';

/** Accès patient / DMP (cf. specs/features/F02). PIN requis côté DMP. */
@Injectable({ providedIn: 'root' })
export class PatientsApi {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  /** GET /api/patients/{ins}/dmp — test d'existence du DMP (TD 0.2). */
  checkDmpExists(ins: string, insOid: string = InsOid.Nir): Observable<DmpExistence> {
    const params = new HttpParams().set('insOid', insOid);
    return this.http.get<DmpExistence>(
      `${this.base}/api/patients/${encodeURIComponent(ins)}/dmp`,
      { params },
    );
  }
}
