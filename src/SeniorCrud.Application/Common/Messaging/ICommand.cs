using MediatR;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Common.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResult> : IRequest<Result<TResult>>
{
}
