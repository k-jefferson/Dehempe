using Dehempe.Application.Common.Exceptions;
using Dehempe.Domain.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace Dehempe.API.Middleware;

internal sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            await WriteProblem(context, StatusCodes.Status400BadRequest, "Validation échouée.", errors);
        }
        catch (DmpPatientNotFoundException ex)
        {
            await WriteDmpProblem(context, StatusCodes.Status404NotFound, ex.Message, ex);
        }
        catch (DmpDocumentNotFoundException ex)
        {
            await WriteDmpProblem(context, StatusCodes.Status404NotFound, ex.Message, ex);
        }
        catch (DmpAuthException ex)
        {
            _logger.LogError(ex, "Erreur d'authentification DMP");
            await WriteDmpProblem(context, StatusCodes.Status502BadGateway, ex.Message, ex);
        }
        catch (DmpException ex)
        {
            _logger.LogError(ex, "Erreur DMP [{Code}]", ex.ErrorCode);
            await WriteDmpProblem(context, StatusCodes.Status502BadGateway, ex.Message, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue");
            await WriteProblem(context, StatusCodes.Status500InternalServerError, "Une erreur inattendue est survenue.");
        }
    }

    private static Task WriteProblem(
        HttpContext ctx,
        int statusCode,
        string title,
        object? errors = null)
    {
        ctx.Response.StatusCode  = statusCode;
        ctx.Response.ContentType = "application/problem+json";
        var problem = new { title, status = statusCode, errors };
        return ctx.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    /// <summary>
    /// Réponse d'erreur DMP enrichie : inclut <c>endpoint</c>, <c>soapAction</c>,
    /// <c>request</c> (XML SOAP brut envoyé) et <c>response</c> (réponse brute du DMP)
    /// quand l'exception les porte, pour faciliter le debug applicatif.
    /// </summary>
    private static Task WriteDmpProblem(HttpContext ctx, int statusCode, string title, DmpException ex)
    {
        ctx.Response.StatusCode  = statusCode;
        ctx.Response.ContentType = "application/problem+json";
        var problem = new
        {
            title,
            status     = statusCode,
            errors     = (object?)null,
            errorCode  = ex.ErrorCode,
            endpoint   = ex.Endpoint,
            soapAction = ex.SoapAction,
            request    = ex.RequestBody,
            response   = ex.ResponseBody,
        };
        return ctx.Response.WriteAsync(JsonSerializer.Serialize(problem,
            new JsonSerializerOptions { WriteIndented = false }));
    }
}
