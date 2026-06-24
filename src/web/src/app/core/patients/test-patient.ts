/**
 * Patient du **jeu d'essai DMP local** (`src/data/patients-dmp.json`).
 *
 * Donnée de test **embarquée dans le bundle** — elle ne provient PAS de l'API `Dehempe.API`
 * (cf. `specs/features/F06-patient-list.md`). Les champs reflètent tels quels le fichier source.
 */
export interface TestPatient {
  idPatient: number;
  prenomDeNaissance: string | null;
  listePrenoms: string | null;
  prenomUtilise: string | null;
  nomDeNaissance: string | null;
  nomUtilise: string | null;
  /** INS/NIR : nombre, chaîne (NIR corse avec lettre) ou `null` selon les entrées du jeu d'essai. */
  matriculeInsNir: number | string | null;
  /** Date ISO `yyyy-MM-dd`. */
  dateDeNaissance: string | null;
  rangDeNaissance: number | null;
  /** `"M"` / `"F"` (autres valeurs tolérées → « Sexe non précisé »). */
  sexe: string | null;
  carteCvTestInsCNumeroBeneficiaire: string | null;
  statutDmp: string | null;
  commentaireRemarques: string | null;
  phase: string | null;
  homSimp: string | null;
}
