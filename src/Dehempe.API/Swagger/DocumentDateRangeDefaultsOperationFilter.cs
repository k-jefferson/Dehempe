using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dehempe.API.Swagger;

/// <summary>
/// Pré-remplit dans Swagger UI les bornes de date du listing de documents
/// (<c>GET /api/patients/{ins}/documents</c>) :
/// <c>createdAfter</c> = aujourd'hui − 30 jours, <c>createdBefore</c> = aujourd'hui.
///
/// Les valeurs sont recalculées à chaque génération du document Swagger, elles restent
/// donc toujours « du jour ». Le filtre ne touche que les opérations qui exposent
/// effectivement ces paramètres ; les autres sont ignorées.
/// </summary>
internal sealed class DocumentDateRangeDefaultsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters is null) return;

        var now = DateTimeOffset.Now;
        SetQueryDefault(operation, "createdAfter",  now.AddDays(-30));
        SetQueryDefault(operation, "createdBefore", now);
    }

    private static void SetQueryDefault(OpenApiOperation operation, string name, DateTimeOffset value)
    {
        var parameter = operation.Parameters.FirstOrDefault(
            p => p.In == ParameterLocation.Query && string.Equals(p.Name, name, StringComparison.Ordinal));
        if (parameter?.Schema is null) return;

        parameter.Schema.Default = new OpenApiString(value.ToString("yyyy-MM-dd'T'HH:mm:sszzz"));
    }
}
