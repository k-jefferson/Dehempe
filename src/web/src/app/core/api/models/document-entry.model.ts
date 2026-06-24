/**
 * Métadonnées d'un document DMP (ITI-18) — calqué sur DocumentEntryDto.
 * Voir specs/architecture/api-contract.md.
 */
export interface DocumentEntry {
  uniqueId: string;
  repositoryUniqueId: string;
  homeCommunityId: string | null;
  title: string | null;
  /** 'APPROVED' | 'DEPRECATED'. */
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

/** Statuts de document XDS. */
export const DocumentStatus = {
  Approved: 'APPROVED',
  Deprecated: 'DEPRECATED',
} as const;

/** Filtres de la liste de documents (cf. DocumentsController.GetDocuments). */
export interface DocumentListFilter {
  insOid?: string;
  createdAfter?: string;
  createdBefore?: string;
  status?: string;
  classCode?: string[];
}
