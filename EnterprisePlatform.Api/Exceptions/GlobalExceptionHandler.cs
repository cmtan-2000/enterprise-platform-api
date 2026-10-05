using EnterprisePlatform.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EnterprisePlatform.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException =>
                CreateValidationProblem(validationException, httpContext),

            NotFoundException notFoundException =>
                CreateProblem(
                    StatusCodes.Status404NotFound,
                    "Resource not found.",
                    notFoundException.Message,
                    notFoundException.Code,
                    httpContext),

            ConflictException conflictException =>
                CreateProblem(
                    StatusCodes.Status409Conflict,
                    "Resource conflict.",
                    conflictException.Message,
                    conflictException.Code,
                    httpContext),

            _ => CreateProblem(
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                null,
                null,
                httpContext)
        };

        if (problemDetails.Status >= 500)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Request failed. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode =
            problemDetails.Status!.Value;

        await _problemDetailsService.WriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });

        return true;
    }

    private static ValidationProblemDetails CreateValidationProblem(
        ValidationException exception,
        HttpContext httpContext)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        return problem;
    }

    private static ProblemDetails CreateProblem(
        int status,
        string title,
        string? detail,
        string? code,
        HttpContext httpContext)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        if (code is not null)
        {
            problem.Extensions["code"] = code;
        }

        return problem;
    }
}