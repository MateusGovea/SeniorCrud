using FluentAssertions;
using SeniorCrud.Domain.Entities;
using SeniorCrud.Domain.Enums;
using SeniorCrud.Domain.Exceptions;
using SeniorCrud.Domain.ValueObjects;
using SeniorCrud.UnitTests.Common;

namespace SeniorCrud.UnitTests.Domain;

public class EntitiesTests
{
    [Fact]
    public void User_Should_Update_Profile()
    {
        var user = TestDataFactory.CreateUser();
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        user.UpdateProfile(
            "Updated Name",
            new Email("updated@test.com"),
            new Cpf("98765432100"),
            new DateOnly(1991, 2, 2),
            updatedAt);

        user.Name.Should().Be("Updated Name");
        user.Email.Value.Should().Be("updated@test.com");
        user.Cpf!.Value.Should().Be("98765432100");
        user.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void User_Should_Throw_When_Authenticating_Inactive()
    {
        var user = TestDataFactory.CreateUser(isActive: false);

        var action = user.EnsureCanAuthenticate;

        action.Should().Throw<InvalidUserStateException>();
    }

    [Fact]
    public void User_Should_Manage_Primary_Address()
    {
        var user = TestDataFactory.CreateUser();
        var first = TestDataFactory.CreateAddress(userId: user.Id, isPrimary: true);
        var second = TestDataFactory.CreateAddress(userId: user.Id, isPrimary: true);
        var now = DateTimeOffset.UtcNow.AddMinutes(1);

        user.AddAddress(first, now);
        user.AddAddress(second, now.AddMinutes(1));

        user.Addresses.Count(a => a.IsPrimary).Should().Be(1);
        user.Addresses.Single(a => a.IsPrimary).Id.Should().Be(second.Id);
    }

    [Fact]
    public void Address_Should_Update_And_Toggle_Primary()
    {
        var address = TestDataFactory.CreateAddress();
        var now = DateTimeOffset.UtcNow.AddMinutes(3);

        address.Update(
            new Cep("30140071"),
            "Av Brasil",
            new AddressNumber("55"),
            null,
            "Centro",
            "Belo Horizonte",
            "MG",
            now);
        address.SetAsPrimary(now.AddMinutes(1));

        address.Cep.Value.Should().Be("30140071");
        address.Street.Should().Be("Av Brasil");
        address.Number.Value.Should().Be("55");
        address.State.Should().Be("MG");
        address.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void Address_Should_Throw_When_State_Invalid()
    {
        var action = () => new Address(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new Cep("01001000"),
            "Rua X",
            new AddressNumber("10"),
            null,
            "Centro",
            "Sao Paulo",
            "SP1",
            false,
            DateTimeOffset.UtcNow);

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void User_Should_Change_Role()
    {
        var user = TestDataFactory.CreateUser(role: UserRole.User);

        user.ChangeRole(UserRole.Admin, DateTimeOffset.UtcNow.AddMinutes(1));

        user.Role.Should().Be(UserRole.Admin);
    }
}
