using SeniorCrud.Domain.Exceptions;

namespace SeniorCrud.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Entity id cannot be empty.");
        }

        Id = id;
    }

    protected Entity()
    {
    }
}
