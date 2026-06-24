import { Injectable, signal } from '@angular/core';
import patientsJson from '../../../data/patients-dmp.json';
import { TestPatient } from './test-patient';

/**
 * Expose le **jeu d'essai patients LOCAL** (hors API), cf. `specs/features/F06-patient-list.md`.
 *
 * La donnée est **statique et embarquée** dans le bundle (import JSON) : aucun appel réseau,
 * aucune persistance disque, aucun PIN. C'est volontairement un service `core/patients` distinct
 * de `core/api/*` (ce n'est pas un endpoint backend).
 */
@Injectable({ providedIn: 'root' })
export class PatientsDataset {
  /** Patients de test, **dans l'ordre du fichier source** (aucun tri appliqué). */
  readonly patients = signal<readonly TestPatient[]>(patientsJson as readonly TestPatient[]);
}
