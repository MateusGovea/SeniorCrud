using SeniorCrud.Domain.Entities;

namespace SeniorCrud.Application.Interfaces.Persistence;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Address>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Address address, CancellationToken cancellationToken = default);

    void Update(Address address);

    void Remove(Address address);

    IQueryable<Address> Query();
}
