using MediatR;
using Microsoft.Extensions.Logging;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Behaviors;

public sealed class ExceptionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
    where TResult : IResult
{
    private readonly ILogger<ExceptionBehavior<TRequest, TResult>> _logger;

    public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception while processing request {RequestName}", typeof(TRequest).Name);
            return ResultFactory.CreateFailure<TResult>(new Error("Application.Unhandled", "An unexpected application error occurred.", ErrorType.Failure));
        }
    }
}
