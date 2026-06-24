/**
 * Environnement de PRODUCTION.
 * En prod, le SPA peut être servi par l'API (même origine → apiBaseUrl = '').
 */
export const environment = {
  production: true,
  /** Préfixe des appels API. '' = même origine. Sinon URL absolue de Dehempe.API. */
  apiBaseUrl: '',
  /** Clé d'API (en-tête X-Api-Key). Laisser vide si l'API ne l'exige pas. */
  apiKey: '',
};
