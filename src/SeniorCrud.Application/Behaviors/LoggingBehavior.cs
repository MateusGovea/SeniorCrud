using MediatR;
using Microsoft.Extensions.Logging;
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
        _logger.LogInformation("Starting request {RequestName}", requestName);

        var result = await next();

        if (result.IsSuccess)
        {
            _logger.LogInformation("Finished request {RequestName} with success", requestName);
        }
        else
        {
            _logger.LogWarning("Finished request {RequestName} with failure {ErrorCode}", requestName, result.Error.Code);
        }

        return result;
    }
}
