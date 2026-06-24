import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CpsCard } from './models';

/** Accès à la carte CPS (cf. specs/features/F01). */
@Injectable({ providedIn: 'root' })
export class CpsApi {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  /** GET /api/cps/card — identité du porteur + carte. Pas de PIN. */
  getCard(): Observable<CpsCard> {
    return this.http.get<CpsCard>(`${this.base}/api/cps/card`);
  }
}
