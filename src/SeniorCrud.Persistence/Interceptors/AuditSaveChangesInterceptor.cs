using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SeniorCrud.Domain.Common;

namespace SeniorCrud.Persistence.Interceptors;

public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyCreatedAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyCreatedAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplyCreatedAudit(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State != EntityState.Added)
            {
                continue;
            }

            if (entry.Entity.CreatedAt == default)
            {
                entry.Property(entity => entity.CreatedAt).CurrentValue = utcNow;
            }
        }
    }
}
