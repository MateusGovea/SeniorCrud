using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
    where TResult : IResult
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResult>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await next();
        stopwatch.Stop();

        _logger.LogInformation("Request {RequestName} took {ElapsedMilliseconds} ms", typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

        return result;
    }
}
