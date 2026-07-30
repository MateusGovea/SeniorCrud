using SeniorCrud.Application.Abstractions.DateTime;

namespace SeniorCrud.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
