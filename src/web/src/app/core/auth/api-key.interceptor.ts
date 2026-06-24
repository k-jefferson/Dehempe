import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';

/**
 * Ajoute l'en-tête X-Api-Key UNIQUEMENT si une clé est configurée
 * (cf. specs/architecture/auth-and-pin-flow.md). Désactivé en dev (clé vide).
 */
export const apiKeyInterceptor: HttpInterceptorFn = (req, next) => {
  const key = environment.apiKey;
  if (!key) {
    return next(req);
  }
  return next(req.clone({ setHeaders: { 'X-Api-Key': key } }));
};
