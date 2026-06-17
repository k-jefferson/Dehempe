using Microsoft.Extensions.Options;

namespace Dehempe.API.Middleware;

/// <summary>
/// Protection minimale de l'API par clé statique dans l'en-tête X-Api-Key.
/// Si ApiKeyOptions.ApiKey est vide ou absent, toutes les requêtes passent (mode dev).
/// </summary>
internal sealed class ApiKeyMiddleware
{
    private const string HeaderName = "X-Api-Key";

    private readonly RequestDelegate _next;
    private readonly string? _expectedKey;

    public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options)
    {
        _next        = next;
        _expectedKey = options.Value.ApiKey;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Si aucune clé configurée, on laisse passer (utile en dev)
        if (string.IsNullOrWhiteSpace(_expectedKey))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var provided)
            || provided != _expectedKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Clé API manquante ou invalide." });
            return;
        }

        await _next(context);
    }
}

public sealed class ApiKeyOptions
{
    public const string SectionName = "ApiKey";
    public string? ApiKey { get; set; }
}
