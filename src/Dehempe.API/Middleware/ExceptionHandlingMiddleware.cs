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
        _next = next;
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
            await WriteProblem(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DmpDocumentNotFoundException ex)
        {
            await WriteProblem(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DmpAuthException ex)
        {
            _logger.LogError(ex, "Erreur d'authentification DMP");
            await WriteProblem(context, StatusCodes.Status502BadGateway, ex.Message);
        }
        catch (DmpException ex)
        {
            _logger.LogError(ex, "Erreur DMP [{Code}]", ex.ErrorCode);
            await WriteProblem(context, StatusCodes.Status502BadGateway, ex.Message);
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
        ctx.Response.StatusCode = statusCode;
        ctx.Response.ContentType = "application/problem+json";
        var problem = new { title, status = statusCode, errors };
        return ctx.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
