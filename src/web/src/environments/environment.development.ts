/**
 * Environnement de DÉVELOPPEMENT.
 * apiBaseUrl vide → appels relatifs `/api/...` redirigés par proxy.conf.json
 * vers https://localhost:7270 (profil https de Dehempe.API).
 */
export const environment = {
  production: false,
  apiBaseUrl: '',
  apiKey: '',
};
