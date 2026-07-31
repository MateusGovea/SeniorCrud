using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace SeniorCrud.Api.Configurations;

/// <summary>
/// Strongly-typed configuration for rate limiting, bound from the "RateLimiting" section.
/// Sensible production defaults are provided so the application works even without configuration.
/// </summary>
public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public FixedWindowOptions Global { get; init; } = new();

    public FixedWindowOptions Login { get; init; } = new()
    {
        PermitLimit = 10
    };

    public void EnsureValid()
    {
        Global.EnsureValid($"{SectionName}:Global");
        Login.EnsureValid($"{SectionName}:Login");
    }
}

public sealed class FixedWindowOptions
{
    public int PermitLimit { get; init; } = 100;

    public int WindowSeconds { get; init; } = 60;

    public int QueueLimit { get; init; }

    public QueueProcessingOrder QueueProcessingOrder { get; init; } = QueueProcessingOrder.OldestFirst;

    public bool AutoReplenishment { get; init; } = true;

    public FixedWindowRateLimiterOptions Create() => new()
    {
        PermitLimit = PermitLimit,
        Window = TimeSpan.FromSeconds(WindowSeconds),
        QueueLimit = QueueLimit,
        QueueProcessingOrder = QueueProcessingOrder,
        AutoReplenishment = AutoReplenishment
    };

    public void EnsureValid(string sectionPath)
    {
        if (PermitLimit <= 0)
        {
            throw new InvalidOperationException($"{sectionPath}:PermitLimit must be greater than zero.");
        }

        if (WindowSeconds <= 0)
        {
            throw new InvalidOperationException($"{sectionPath}:WindowSeconds must be greater than zero.");
        }

        if (QueueLimit < 0)
        {
            throw new InvalidOperationException($"{sectionPath}:QueueLimit cannot be negative.");
        }
    }
}
