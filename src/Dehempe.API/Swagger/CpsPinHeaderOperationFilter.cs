using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dehempe.API.Swagger;

/// <summary>
/// Ajoute un paramètre d'en-tête optionnel <c>X-Cps-Pin</c> à toutes les opérations dans Swagger UI,
/// afin de pouvoir saisir le code porteur de la carte CPS directement depuis Swagger lors d'un test.
///
/// En production, ce header est transmis par le frontend (voir <c>Pkcs11CpsKeyStore.PinHeaderName</c>).
/// </summary>
internal sealed class CpsPinHeaderOperationFilter : IOperationFilter
{
    private const string HeaderName = "X-Cps-Pin";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            Name        = HeaderName,
            In          = ParameterLocation.Header,
            Required    = false,
            Description = "Code porteur de la carte CPS. Laisser vide pour être invité par un dialog " +
                          "natif sur le poste (si Cps:InteractivePinPrompt = true).",
            Schema      = new OpenApiSchema { Type = "string", Format = "password" }
        });
    }
}
