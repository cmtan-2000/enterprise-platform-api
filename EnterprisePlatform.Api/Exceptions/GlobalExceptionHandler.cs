using EnterprisePlatform.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePlatform.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "An exception occurred. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        ProblemDetails problemDetails = exception switch
        {
            ValidationException validationException
                => CreateValidationProblem(
                    httpContext,
                    validationException),

            NotFoundException notFoundException
                => CreateNotFoundProblem(
                    httpContext,
                    notFoundException),

            ConflictException conflictException
                => CreateConflictProblem(
                    httpContext,
                    conflictException),

            DbUpdateConcurrencyException
                => CreateConcurrencyProblem(
                    httpContext),

            _ => CreateInternalServerError(
                httpContext)
        };

        httpContext.Response.StatusCode =
            problemDetails.Status
            ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }

    private static ProblemDetails CreateValidationProblem(HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(x => x.ErrorMessage)
                    .ToArray());

        return new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed.",
            Type = "https://httpstatuses.com/400",
            Extensions =
            {
                ["code"] = "validation.failed",
                ["traceId"] = context.TraceIdentifier,
                ["errors"] = errors
            }
        };
    }

    private static ProblemDetails CreateNotFoundProblem(HttpContext context, NotFoundException exception)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Resource not found.",
            Type = "https://httpstatuses.com/404",
            Detail = exception.Message,
            Extensions =
            {
                ["code"] = "resource.not_found",
                ["traceId"] = context.TraceIdentifier
            }
        };
    }

    private static ProblemDetails CreateConflictProblem(HttpContext context, ConflictException exception)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict occurred.",
            Type = "https://httpstatuses.com/409",
            Detail = exception.Message,
            Extensions =
            {
                ["code"] = "resource.conflict",
                ["traceId"] = context.TraceIdentifier
            }
        };
    }

    private static ProblemDetails CreateInternalServerError(HttpContext context)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://httpstatuses.com/500",
            Extensions =
            {
                ["code"] = "internal_server_error",
                ["traceId"] = context.TraceIdentifier
            }
        };
    }

    private static ProblemDetails CreateConcurrencyProblem(HttpContext context)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Concurrency conflict.",
            Type = "https://httpstatuses.com/409",
            Detail = "The resource was modified by another request. Please retrieve the latest version and try again.",
            Instance = context.Request.Path,
            Extensions =
        {
            ["code"] = "resource.concurrency_conflict",
            ["traceId"] = context.TraceIdentifier
        }
        };
    }
}