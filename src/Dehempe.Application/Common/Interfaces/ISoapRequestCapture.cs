namespace Dehempe.Application.Common.Interfaces;

/// <summary>
/// Capture scopée de la dernière requête SOAP envoyée au DMP dans le cadre d'une requête HTTP.
/// Permet d'exposer le XML brut même quand le DMP répond en HTTP 200.
/// </summary>
public interface ISoapRequestCapture
{
    string? LastRequest { get; set; }
}
