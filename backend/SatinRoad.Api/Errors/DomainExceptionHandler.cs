using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SatinRoad.Core.Common;

namespace SatinRoad.Api.Errors;

public class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken ct)
    {
        var status = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException   => StatusCodes.Status404NotFound,
            ConflictException   => StatusCodes.Status409Conflict,
            _ => 0
        };

        if (status == 0) return false; 

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = status, Title = exception.Message }, ct);
        return true;
    }
}