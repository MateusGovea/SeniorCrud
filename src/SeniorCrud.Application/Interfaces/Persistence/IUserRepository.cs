using SeniorCrud.Domain.Entities;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.Application.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    Task<User?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(Email email, Guid? excludingUserId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCpfAsync(Cpf cpf, Guid? excludingUserId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetPagedAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    void Update(User user);

    void Remove(User user);

    IQueryable<User> Query();
}
