using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SeniorCrud.Domain.Common;

namespace SeniorCrud.Persistence.Interceptors;

public sealed class SoftDeleteSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplySoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplySoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplySoftDelete(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entry.Property("IsDeleted").CurrentValue = true;
            entry.Property("DeletedAt").CurrentValue = utcNow;
            entry.Property(entity => entity.UpdatedAt).CurrentValue = utcNow;
        }
    }
}
