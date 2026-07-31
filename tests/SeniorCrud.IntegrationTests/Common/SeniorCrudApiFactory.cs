using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SeniorCrud.Application.Abstractions.Authentication;
using SeniorCrud.Application.Abstractions.ViaCep;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Domain.Entities;
using SeniorCrud.Domain.Enums;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.IntegrationTests.Common;

public sealed class SeniorCrudApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IUserRepository>();
            services.RemoveAll<IAddressRepository>();
            services.RemoveAll<IUnitOfWork>();
            services.RemoveAll<IViaCepClient>();

            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            services.AddSingleton<IAddressRepository, InMemoryAddressRepository>();
            services.AddSingleton<IUnitOfWork, NoOpUnitOfWork>();
            services.AddSingleton<IViaCepClient, StubViaCepClient>();
        });
    }

    private sealed class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public InMemoryUserRepository(IPasswordHasher passwordHasher)
        {
            _users = [SeedAdmin(passwordHasher)];
        }

        private static User SeedAdmin(IPasswordHasher passwordHasher)
        {
            return new User(
                Guid.NewGuid(),
                "Admin Test",
                new Email("admin@test.com"),
                new PasswordHash(passwordHasher.HashPassword("admin123")),
                UserRole.Admin,
                DateTimeOffset.UtcNow);
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.FirstOrDefault(user => user.Id == id));

        public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.FirstOrDefault(user => user.Email == email));

        public Task<User?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.FirstOrDefault(user => user.Cpf == cpf));

        public Task<bool> ExistsByEmailAsync(Email email, Guid? excludingUserId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.Any(user => user.Email == email && (!excludingUserId.HasValue || user.Id != excludingUserId.Value)));

        public Task<bool> ExistsByCpfAsync(Cpf cpf, Guid? excludingUserId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.Any(user => user.Cpf == cpf && (!excludingUserId.HasValue || user.Id != excludingUserId.Value)));

        public Task<IReadOnlyList<User>> GetPagedAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken = default)
        {
            var query = _users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalized = search.Trim().ToLowerInvariant();
                query = query.Where(user => user.Name.ToLowerInvariant().Contains(normalized) ||
                                            user.Email.Value.ToLowerInvariant().Contains(normalized) ||
                                            (user.Cpf != null && user.Cpf.Value.Contains(normalized)));
            }

            var result = query
                .OrderBy(user => user.Name)
                .Skip((Math.Max(pageNumber, 1) - 1) * Math.Max(pageSize, 1))
                .Take(Math.Max(pageSize, 1))
                .ToList();

            return Task.FromResult((IReadOnlyList<User>)result);
        }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public void Update(User user)
        {
        }

        public void Remove(User user)
        {
            _users.Remove(user);
        }

        public IQueryable<User> Query()
        {
            return _users.AsQueryable();
        }
    }

    private sealed class InMemoryAddressRepository : IAddressRepository
    {
        private readonly List<Address> _addresses = [];

        public Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_addresses.FirstOrDefault(address => address.Id == id));

        public Task<IReadOnlyList<Address>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => Task.FromResult((IReadOnlyList<Address>)_addresses.Where(address => address.UserId == userId).OrderBy(address => address.CreatedAt).ToList());

        public Task<IReadOnlyList<Address>> GetPagedAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken = default)
        {
            var query = _addresses.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(a =>
                    a.Street.ToLower().Contains(s) ||
                    a.City.ToLower().Contains(s) ||
                    a.Neighborhood.ToLower().Contains(s));
            }
            var result = query
                .OrderBy(a => a.Street)
                .ThenBy(a => a.Number.Value)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Task.FromResult((IReadOnlyList<Address>)result);
        }

        public Task AddAsync(Address address, CancellationToken cancellationToken = default)
        {
            _addresses.Add(address);
            return Task.CompletedTask;
        }

        public void Update(Address address)
        {
        }

        public void Remove(Address address)
        {
            _addresses.Remove(address);
        }

        public IQueryable<Address> Query()
        {
            return _addresses.AsQueryable();
        }
    }

    private sealed class NoOpUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class StubViaCepClient : IViaCepClient
    {
        public Task<ViaCepAddressResponse?> GetAddressByCepAsync(string cep, CancellationToken cancellationToken = default)
        {
            var normalized = new string((cep ?? string.Empty).Where(char.IsDigit).ToArray());
            if (normalized != "01001000")
            {
                return Task.FromResult<ViaCepAddressResponse?>(null);
            }

            return Task.FromResult<ViaCepAddressResponse?>(new ViaCepAddressResponse
            {
                Cep = "01001-000",
                Logradouro = "Praca da Se",
                Bairro = "Se",
                Localidade = "Sao Paulo",
                Uf = "SP",
                Ibge = "3550308",
                Ddd = "11",
                Erro = false
            });
        }
    }
}
