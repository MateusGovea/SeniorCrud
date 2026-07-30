using FluentAssertions;
using Moq;
using SeniorCrud.Application.Abstractions.Authentication;
using SeniorCrud.Application.DTOs.Authentication;
using SeniorCrud.Application.Features.Authentication.Commands;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Domain.ValueObjects;
using SeniorCrud.UnitTests.Common;

namespace SeniorCrud.UnitTests.Application.Handlers;

public class AuthenticationHandlersTests
{
    [Fact]
    public async Task LoginCommandHandler_Should_Return_Unauthorized_When_User_Not_Found()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SeniorCrud.Domain.Entities.User?)null);

        var hasher = new Mock<IPasswordHasher>();
        var jwt = new Mock<IJwtTokenGenerator>();

        var handler = new LoginCommandHandler(userRepository.Object, hasher.Object, jwt.Object);

        var result = await handler.Handle(new LoginCommand("user@test.com", "password123"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(SeniorCrud.Application.Results.ErrorType.Unauthorized);
    }

    [Fact]
    public async Task LoginCommandHandler_Should_Return_Unauthorized_When_Password_Invalid()
    {
        var user = TestDataFactory.CreateUser(passwordHash: new string('h', 60));

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.VerifyPassword("wrong", user.PasswordHash.Value)).Returns(false);

        var jwt = new Mock<IJwtTokenGenerator>();

        var handler = new LoginCommandHandler(userRepository.Object, hasher.Object, jwt.Object);

        var result = await handler.Handle(new LoginCommand(user.Email.Value, "wrong"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Authentication.InvalidCredentials");
    }

    [Fact]
    public async Task LoginCommandHandler_Should_Return_Success_When_Credentials_Are_Valid()
    {
        var user = TestDataFactory.CreateUser();
        var token = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0IiwibmFtZSI6IlRlc3QiLCJleHAiOjQxMDA5MDg4MDB9.sig";

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.VerifyPassword("password123", user.PasswordHash.Value)).Returns(true);

        var jwt = new Mock<IJwtTokenGenerator>();
        jwt.Setup(generator => generator.GenerateAccessToken(user.Id, user.Name, null)).Returns(token);

        var handler = new LoginCommandHandler(userRepository.Object, hasher.Object, jwt.Object);

        var result = await handler.Handle(new LoginCommand(user.Email.Value, "password123"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<LoginResponseDto>();
        result.Value!.AccessToken.Should().Be(token);
    }
}
