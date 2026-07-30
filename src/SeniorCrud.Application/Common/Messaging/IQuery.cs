using MediatR;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Common.Messaging;

public interface IQuery<TResult> : IRequest<Result<TResult>>
{
}
