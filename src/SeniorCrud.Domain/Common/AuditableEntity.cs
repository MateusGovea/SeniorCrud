using SeniorCrud.Domain.Exceptions;

namespace SeniorCrud.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; protected set; }

    public DateTimeOffset UpdatedAt { get; protected set; }

    protected AuditableEntity(Guid id, DateTimeOffset createdAtUtc)
        : base(id)
    {
        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new DomainException("CreatedAt must be in UTC.");
        }

        CreatedAt = createdAtUtc;
        UpdatedAt = createdAtUtc;
    }

    protected AuditableEntity()
    {
    }

    protected void MarkUpdated(DateTimeOffset updatedAtUtc)
    {
        if (updatedAtUtc.Offset != TimeSpan.Zero)
        {
            throw new DomainException("UpdatedAt must be in UTC.");
        }

        if (updatedAtUtc < CreatedAt)
        {
            throw new DomainException("UpdatedAt cannot be earlier than CreatedAt.");
        }

        UpdatedAt = updatedAtUtc;
    }
}
