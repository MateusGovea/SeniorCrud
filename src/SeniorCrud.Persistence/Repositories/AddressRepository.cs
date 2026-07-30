using Microsoft.EntityFrameworkCore;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Domain.Entities;
using SeniorCrud.Persistence.Contexts;

namespace SeniorCrud.Persistence.Repositories;

public sealed class AddressRepository : IAddressRepository
{
    private readonly SeniorCrudDbContext _dbContext;

    public AddressRepository(SeniorCrudDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Address>> GetPagedAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Addresses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();
            query = query.Where(address =>
                address.Street.ToLower().Contains(normalizedSearch) ||
                address.City.ToLower().Contains(normalizedSearch) ||
                address.Neighborhood.ToLower().Contains(normalizedSearch));
        }

        return await query
            .OrderBy(address => address.Street)
            .ThenBy(address => address.Number.Value)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Addresses
            .FirstOrDefaultAsync(address => address.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Address>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Addresses
            .Where(address => address.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Address address, CancellationToken cancellationToken = default)
    {
        await _dbContext.Addresses.AddAsync(address, cancellationToken);
    }

    public void Update(Address address)
    {
        _dbContext.Addresses.Update(address);
    }

    public void Remove(Address address)
    {
        _dbContext.Addresses.Remove(address);
    }

    public IQueryable<Address> Query()
    {
        return _dbContext.Addresses.AsQueryable();
    }
}
