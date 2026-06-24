/**
 * Forme `application/problem+json` renvoyée par ExceptionHandlingMiddleware (Dehempe.API).
 * Voir specs/architecture/api-contract.md (§ Contrat d'erreur).
 */
export interface ApiProblem {
  title?: string;
  status?: number;
  errors?: Record<string, string[]> | null;
  /** Code d'erreur DMP (ex. 'CpsPinRequired'). */
  errorCode?: string;
  /** Champs de debug — ne pas afficher par défaut (peuvent contenir des données patient). */
  endpoint?: string;
  soapAction?: string;
  request?: string;
  response?: string;
  /** Présent sur l'erreur de clé d'API : { error: 'Clé API…' }. */
  error?: string;
}

/** Code d'erreur signalant qu'un PIN CPS est requis (cf. auth-and-pin-flow.md). */
export const PIN_REQUIRED_CODE = 'CpsPinRequired';
