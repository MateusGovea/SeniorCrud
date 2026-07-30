using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SeniorCrud.Application.Common.Diagnostics;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
    where TResult : IResult
{
    private readonly ILogger<LoggingBehavior<TRequest, TResult>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        using var activity = ApplicationActivitySource.Source.StartActivity(requestName, ActivityKind.Internal);
        activity?.SetTag("app.request.name", requestName);
        activity?.SetTag("app.request.type", typeof(TRequest).FullName);

        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting use case {RequestName}", requestName);

        var result = await next();
        stopwatch.Stop();

        activity?.SetTag("app.request.success", result.IsSuccess);
        activity?.SetTag("app.request.duration.ms", stopwatch.ElapsedMilliseconds);

        if (!result.IsSuccess)
        {
            activity?.SetTag("app.request.error.code", result.Error.Code);
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Finished use case {RequestName} with success in {ElapsedMilliseconds} ms", requestName, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogWarning("Finished use case {RequestName} with failure {ErrorCode} in {ElapsedMilliseconds} ms", requestName, result.Error.Code, stopwatch.ElapsedMilliseconds);
        }

        return result;
    }
}
