namespace Dehempe.Application.Cps.DTOs;

public record CpsCardDto(
    CpsPorteurDto  Porteur,
    CpsCarteDto    Carte
);

public record CpsPorteurDto(
    string Nom,
    string Prenom,
    string Identifiant,
    string Profession
);

public record CpsCarteDto(
    string   Numero,
    DateOnly DateEmission,
    DateOnly DateExpiration
);
