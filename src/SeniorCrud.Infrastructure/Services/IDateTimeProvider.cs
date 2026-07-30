namespace SeniorCrud.Infrastructure.Services;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
