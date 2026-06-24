/**
 * Test d'existence du DMP (TD 0.2) — calqué sur DmpExistenceDto / DmpPatientDto.
 * Voir specs/architecture/api-contract.md.
 */
export interface DmpExistence {
  patientIns: string;
  exists: boolean;
  queryResponseCode: string;
  ackTypeCode: string;
  isAuthorizationValid: boolean | null;
  isAttachedToTreatingPhysician: boolean | null;
  patient: DmpPatient | null;
  errorMessage: string | null;
  /** XML SOAP brut — DEBUG uniquement, masqué par défaut dans l'UI. */
  request: string | null;
}

export interface DmpPatient {
  insC: string | null;
  insNir: string | null;
  status: string | null;
  prefix: string | null;
  givenName: string | null;
  familyName: string | null;
  email: string | null;
  phone: string | null;
  genderCode: string | null;
  /** 'yyyy-MM-dd'. */
  birthDate: string | null;
  hasInternetAccount: boolean | null;
  isAttachedToEns: boolean | null;
}

/** OID des INS (le choix dépend de la longueur de l'INS, pas d'un réglage). */
export const InsOid = {
  Nir: '1.2.250.1.213.1.4.8',
  Nia: '1.2.250.1.213.1.4.9',
} as const;
