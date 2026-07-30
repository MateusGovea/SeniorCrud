namespace SeniorCrud.IntegrationTests.Common;

public class ApiResult
{
    public bool IsSuccess { get; set; }

    public ApiError Error { get; set; } = new();

    public IReadOnlyList<ApiValidationError> ValidationErrors { get; set; } = [];
}

public sealed class ApiResult<TValue> : ApiResult
{
    public TValue? Value { get; set; }
}

public sealed class ApiError
{
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Type { get; set; }
}

public sealed class ApiValidationError
{
    public string PropertyName { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
}
