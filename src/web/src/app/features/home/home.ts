import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';

interface FeaturePreview {
  code: string;
  icon: string;
  title: string;
  description: string;
}

/** Page d'accueil — placeholder de scaffold (aucune feature métier encore implémentée). */
@Component({
  selector: 'app-home',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatCardModule, MatChipsModule, MatIconModule],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {
  readonly features: readonly FeaturePreview[] = [
    { code: 'F01', icon: 'badge', title: 'Carte CPS', description: "Lecture de l'identité du praticien sur la carte insérée." },
    { code: 'F02', icon: 'person_search', title: 'Existence du DMP', description: "Test d'existence du DMP d'un patient à partir de son INS." },
    { code: 'F03', icon: 'folder_shared', title: 'Documents', description: 'Liste des documents du DMP, avec filtres.' },
    { code: 'F04', icon: 'description', title: 'Consultation', description: "Consultation et téléchargement d'un document." },
    { code: 'F05', icon: 'password', title: 'Code PIN', description: 'Saisie sécurisée du PIN de la carte CPS.' },
  ];
}
