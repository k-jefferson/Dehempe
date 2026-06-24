import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PatientsDataset } from '../../core/patients/patients-dataset';

type Sexe = 'M' | 'F' | 'other';

/** Ligne d'affichage dérivée d'un `TestPatient` (calculée une fois). */
interface PatientRow {
  id: number;
  nom: string;
  prenom: string;
  dateDeNaissance: string | null;
  sexe: Sexe;
  sexIcon: string;
  sexLabel: string;
  /** JSON complet du patient, indenté (infobulle). */
  json: string;
  /** Champ normalisé (nom + prénom) pour la recherche insensible casse/accents. */
  haystack: string;
}

/** Minuscule + suppression des diacritiques, pour une recherche tolérante. */
function normalize(value: string): string {
  return value
    .normalize('NFD')
    .replace(/\p{Diacritic}/gu, '')
    .toLowerCase();
}

/**
 * Liste des patients du jeu d'essai local, rendue dans le sidenav (cf. `specs/features/F06-patient-list.md`).
 * Affiche / filtre / colore selon le sexe ; aucune sélection ni navigation en v1.
 */
@Component({
  selector: 'app-patient-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe,
    MatListModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatTooltipModule,
  ],
  templateUrl: './patient-list.html',
  styleUrl: './patient-list.scss',
})
export class PatientList {
  private readonly dataset = inject(PatientsDataset);

  /** Texte saisi dans le champ de recherche. */
  readonly query = signal('');

  /** Lignes d'affichage, dans l'ordre du fichier (calculées une seule fois sur la donnée statique). */
  private readonly rows = computed<readonly PatientRow[]>(() =>
    this.dataset.patients().map((p) => {
      const nom = p.nomUtilise ?? p.nomDeNaissance ?? '';
      const prenom = p.prenomUtilise ?? p.prenomDeNaissance ?? '';
      const sexe: Sexe = p.sexe === 'M' ? 'M' : p.sexe === 'F' ? 'F' : 'other';
      return {
        id: p.idPatient,
        nom,
        prenom,
        dateDeNaissance: p.dateDeNaissance,
        sexe,
        sexIcon: sexe === 'M' ? 'male' : sexe === 'F' ? 'female' : 'person',
        sexLabel: sexe === 'M' ? 'Homme' : sexe === 'F' ? 'Femme' : 'Sexe non précisé',
        json: JSON.stringify(p, null, 2),
        haystack: normalize(`${nom} ${prenom}`),
      };
    }),
  );

  /** Lignes filtrées par nom/prénom selon la recherche. */
  readonly filtered = computed<readonly PatientRow[]>(() => {
    const q = normalize(this.query().trim());
    const rows = this.rows();
    return q ? rows.filter((r) => r.haystack.includes(q)) : rows;
  });

  onSearch(event: Event): void {
    this.query.set((event.target as HTMLInputElement).value);
  }

  clear(): void {
    this.query.set('');
  }
}
