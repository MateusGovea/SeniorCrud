namespace SeniorCrud.Application.Results;

public static class ResultFactory
{
    public static TResult CreateFailure<TResult>(Error error)
        where TResult : IResult
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (TResult)(IResult)Result.Failure(error);
        }

        if (IsGenericResultType(typeof(TResult)))
        {
            var genericTypeArgument = typeof(TResult).GetGenericArguments()[0];
            var genericMethod = typeof(ResultFactory)
                .GetMethod(nameof(CreateGenericFailure), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(genericTypeArgument);

            return (TResult)genericMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException($"Unsupported result type: {typeof(TResult).FullName}");
    }

    public static TResult CreateValidationFailure<TResult>(IEnumerable<ValidationError> validationErrors)
        where TResult : IResult
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (TResult)(IResult)Result.ValidationFailure(validationErrors);
        }

        if (IsGenericResultType(typeof(TResult)))
        {
            var genericTypeArgument = typeof(TResult).GetGenericArguments()[0];
            var genericMethod = typeof(ResultFactory)
                .GetMethod(nameof(CreateGenericValidationFailure), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(genericTypeArgument);

            return (TResult)genericMethod.Invoke(null, [validationErrors])!;
        }

        throw new InvalidOperationException($"Unsupported result type: {typeof(TResult).FullName}");
    }

    private static bool IsGenericResultType(Type resultType)
    {
        return resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>);
    }

    private static Result<TValue> CreateGenericFailure<TValue>(Error error)
    {
        return Result<TValue>.Failure(error);
    }

    private static Result<TValue> CreateGenericValidationFailure<TValue>(IEnumerable<ValidationError> validationErrors)
    {
        return Result<TValue>.ValidationFailure(validationErrors);
    }
}
