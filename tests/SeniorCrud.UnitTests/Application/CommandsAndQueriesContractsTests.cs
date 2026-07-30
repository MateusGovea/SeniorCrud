using FluentAssertions;
using SeniorCrud.Application.Features.Addresses.Commands;
using SeniorCrud.Application.Features.Addresses.Queries;
using SeniorCrud.Application.Features.Authentication.Commands;
using SeniorCrud.Application.Features.Export.Commands;
using SeniorCrud.Application.Features.Users.Commands;
using SeniorCrud.Application.Features.Users.Queries;
using SeniorCrud.Application.Features.ViaCep.Queries;

namespace SeniorCrud.UnitTests.Application;

public class CommandsAndQueriesContractsTests
{
    [Fact]
    public void LoginCommand_Should_Keep_Provided_Values()
    {
        var command = new LoginCommand("user@test.com", "pass");

        command.Email.Should().Be("user@test.com");
        command.Password.Should().Be("pass");
    }

    [Fact]
    public void GetUsersQuery_Should_Use_Defaults()
    {
        var query = new GetUsersQuery();

        query.PageNumber.Should().Be(1);
        query.PageSize.Should().Be(20);
        query.Search.Should().BeNull();
    }

    [Fact]
    public void ExportUsersCsvCommand_Should_Hold_UserIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var command = new ExportUsersCsvCommand(ids);

        command.UserIds.Should().BeEquivalentTo(ids);
    }

    [Fact]
    public void CreateAddressCommand_Should_Keep_Provided_Values()
    {
        var userId = Guid.NewGuid();
        var command = new CreateAddressCommand(userId, "01001000", "Rua", "1", null, "Centro", "Sao Paulo", "SP", true);

        command.UserId.Should().Be(userId);
        command.Cep.Should().Be("01001000");
    }

    [Fact]
    public void ViaCepQuery_Should_Store_Cep()
    {
        var query = new GetAddressByCepQuery("01001000");

        query.Cep.Should().Be("01001000");
    }

    [Fact]
    public void Delete_Commands_Should_Store_Identifiers()
    {
        var userId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        new DeleteUserCommand(userId).UserId.Should().Be(userId);
        new DeleteAddressCommand(addressId).AddressId.Should().Be(addressId);
    }

    [Fact]
    public void UpdateUserCommand_Should_Store_Fields()
    {
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand(userId, "Name", "user@test.com", "39053344705", new DateOnly(1990, 1, 1), true);

        command.UserId.Should().Be(userId);
        command.Email.Should().Be("user@test.com");
    }

    [Fact]
    public void User_And_Address_Queries_Should_Store_Ids()
    {
        var userId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        new GetUserByIdQuery(userId).UserId.Should().Be(userId);
        new GetAddressByIdQuery(addressId).AddressId.Should().Be(addressId);
        new GetAddressesByUserQuery(userId).UserId.Should().Be(userId);
    }
}
