/**
 * Métadonnées d'un document DMP (ITI-18) — calqué sur DocumentEntryDto.
 * Voir specs/architecture/api-contract.md.
 */
export interface DocumentEntry {
  uniqueId: string;
  repositoryUniqueId: string;
  homeCommunityId: string | null;
  title: string | null;
  /** Toujours 'APPROVED' (seul statut interrogé par l'API). */
  status: string;
  classCode: string | null;
  typeCode: string | null;
  formatCode: string | null;
  mimeType: string | null;
  /** ISO 8601. */
  creationTime: string | null;
  serviceStartTime: string | null;
  serviceStopTime: string | null;
  authorInstitution: string | null;
  authorPerson: string | null;
}

/** Filtres de la liste de documents (cf. DocumentsController.GetDocuments). */
export interface DocumentListFilter {
  insOid?: string;
  createdAfter?: string;
  createdBefore?: string;
  classCode?: string[];
}

/**
 * Réponse de la liste de documents — calqué sur DocumentListDto (enveloppe).
 * `dmpRequest` / `dmpResponse` ne sont remplis (XML SOAP brut) que lorsque `documents` est
 * vide, pour diagnostiquer une absence de résultat ; sinon ils valent `null`.
 */
export interface DocumentList {
  documents: DocumentEntry[];
  dmpRequest: string | null;
  dmpResponse: string | null;
}
