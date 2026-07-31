using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using SeniorCrud.Application.Results;
using System.Threading.RateLimiting;

namespace SeniorCrud.Api.Middleware;

/// <summary>
/// Writes the standardized rejection response (HTTP 429) when the rate limit is exceeded,
/// reusing the project's Result contract and informing the client how long to wait.
/// </summary>
public static class RateLimitingRejectionHandler
{
    private const string ResponseCode = "RateLimit.Exceeded";
    private const string ResponseMessage = "Too many requests. Please try again later.";

    public static async ValueTask HandleAsync(OnRejectedContext context, CancellationToken cancellationToken)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString("0");
        }

        var result = Result.Failure(ResponseCode, ResponseMessage);
        await context.HttpContext.Response.WriteAsJsonAsync(result, cancellationToken);
    }
}
