using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SeniorCrud.Domain.Common;

namespace SeniorCrud.Persistence.Interceptors;

public sealed class UpdateTimestampsSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyUpdatedAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyUpdatedAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplyUpdatedAudit(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Property(entity => entity.UpdatedAt).CurrentValue = utcNow;
                entry.Property(entity => entity.CreatedAt).IsModified = false;
            }
        }
    }
}
