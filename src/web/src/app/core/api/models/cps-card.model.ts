/**
 * Carte CPS — calqué sur CpsCardDto (Dehempe.Application/Cps/DTOs).
 * Voir specs/architecture/api-contract.md.
 */
export interface CpsCard {
  porteur: CpsPorteur;
  carte: CpsCarte;
}

export interface CpsPorteur {
  nom: string;
  prenom: string;
  /** Identifiant RPPS-like (≈ 12 chiffres). */
  identifiant: string;
  profession: string;
}

export interface CpsCarte {
  numero: string;
  /** 'yyyy-MM-dd' (DateOnly côté API). */
  dateEmission: string;
  /** 'yyyy-MM-dd'. */
  dateExpiration: string;
}
