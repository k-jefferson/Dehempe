import {
  HttpErrorResponse,
  HttpEvent,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, catchError, switchMap, throwError } from 'rxjs';
import { ApiProblem, PIN_REQUIRED_CODE } from '../api/models';
import { PinStore } from './pin.store';

/** En-tête transportant le PIN CPS (constante backend Pkcs11CpsKeyStore.PinHeaderName). */
export const PIN_HEADER = 'X-Cps-Pin';

/** Une 401 « PIN requis » : errorCode dédié, ou en-tête WWW-Authenticate: CpsPin. */
function isPinRequired(err: unknown): boolean {
  if (!(err instanceof HttpErrorResponse) || err.status !== 401) {
    return false;
  }
  const wwwAuth = err.headers?.get('WWW-Authenticate') ?? '';
  if (wwwAuth.includes('CpsPin')) {
    return true;
  }
  const body = err.error as ApiProblem | string | null;
  return !!body && typeof body === 'object' && body.errorCode === PIN_REQUIRED_CODE;
}

function withPin<T>(req: HttpRequest<T>, pin: string): HttpRequest<T> {
  return req.clone({ setHeaders: { [PIN_HEADER]: pin } });
}

/**
 * Flux PIN CPS (cf. specs/architecture/auth-and-pin-flow.md & features/F05) :
 *  - injecte le PIN mémorisé sur chaque requête sortante ;
 *  - sur 401 CpsPinRequired, ouvre le dialog (un seul à la fois) puis rejoue UNE fois ;
 *  - si le rejeu échoue encore en CpsPinRequired → purge le PIN (refusé / carte retirée) ;
 *  - une 401 de clé d'API (sans errorCode) n'ouvre pas le dialog.
 */
export const pinInterceptor: HttpInterceptorFn = (req, next) => {
  const pinStore = inject(PinStore);

  const memorized = pinStore.pin();
  const request = memorized && !req.headers.has(PIN_HEADER) ? withPin(req, memorized) : req;

  return next(request).pipe(
    catchError((err: unknown) => {
      if (!isPinRequired(err)) {
        return throwError(() => err);
      }
      return pinStore.requestPin().pipe(
        switchMap((entered): Observable<HttpEvent<unknown>> => {
          if (!entered) {
            return throwError(() => err); // annulé par l'utilisateur
          }
          return next(withPin(request, entered)).pipe(
            catchError((retryErr: unknown) => {
              if (isPinRequired(retryErr)) {
                pinStore.clear();
              }
              return throwError(() => retryErr);
            }),
          );
        }),
      );
    }),
  );
};
