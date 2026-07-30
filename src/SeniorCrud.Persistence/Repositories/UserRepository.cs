using Microsoft.EntityFrameworkCore;
using SeniorCrud.Domain.Entities;
using SeniorCrud.Domain.ValueObjects;
using SeniorCrud.Persistence.Contexts;

namespace SeniorCrud.Persistence.Repositories;

public sealed class UserRepository
{
    private readonly SeniorCrudDbContext _dbContext;

    public UserRepository(SeniorCrudDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(user => user.Addresses)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(user => user.Addresses)
            .FirstOrDefaultAsync(user => user.Email.Value == email.Value, cancellationToken);
    }

    public async Task<User?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(user => user.Addresses)
            .FirstOrDefaultAsync(user => user.Cpf != null && user.Cpf.Value == cpf.Value, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

    public void Remove(User user)
    {
        _dbContext.Users.Remove(user);
    }

    public IQueryable<User> Query()
    {
        return _dbContext.Users.AsQueryable();
    }
}
