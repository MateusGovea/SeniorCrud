using AutoMapper;
using FluentAssertions;
using Moq;
using SeniorCrud.Application.Abstractions.Authentication;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Features.Users.Commands;
using SeniorCrud.Application.Features.Users.Queries;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Domain.Entities;
using SeniorCrud.Domain.ValueObjects;
using SeniorCrud.UnitTests.Common;

namespace SeniorCrud.UnitTests.Application.Handlers;

public class UsersHandlersTests
{
    private readonly IMapper _mapper = TestDataFactory.CreateMapper();

    [Fact]
    public async Task CreateUserCommandHandler_Should_Return_Conflict_When_Email_Exists()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var unitOfWork = new Mock<IUnitOfWork>();
        var hasher = new Mock<IPasswordHasher>();
        var cache = new Mock<ICacheService>();

        var handler = new CreateUserCommandHandler(userRepository.Object, unitOfWork.Object, hasher.Object, cache.Object, _mapper);

        var result = await handler.Handle(new CreateUserCommand("User", "user@test.com", "password123", null, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(SeniorCrud.Application.Results.ErrorType.Conflict);
    }

    [Fact]
    public async Task CreateUserCommandHandler_Should_Create_User_When_Valid()
    {
        User? createdUser = null;

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        userRepository.Setup(r => r.ExistsByCpfAsync(It.IsAny<Cpf>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        userRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => createdUser = user)
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.HashPassword("password123")).Returns(new string('h', 60));

        var cache = new Mock<ICacheService>();

        var handler = new CreateUserCommandHandler(userRepository.Object, unitOfWork.Object, hasher.Object, cache.Object, _mapper);

        var result = await handler.Handle(new CreateUserCommand("New User", "new@test.com", "password123", "39053344705", null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        createdUser.Should().NotBeNull();
        cache.Verify(c => c.Remove(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateUserCommandHandler_Should_Return_NotFound_When_User_Missing()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var handler = new UpdateUserCommandHandler(userRepository.Object, Mock.Of<IUnitOfWork>(), Mock.Of<ICacheService>(), _mapper);

        var result = await handler.Handle(new UpdateUserCommand(Guid.NewGuid(), "Name", "user@test.com", null, null, true), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(SeniorCrud.Application.Results.ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteUserCommandHandler_Should_Remove_User_When_Exists()
    {
        var user = TestDataFactory.CreateUser();
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var cache = new Mock<ICacheService>();

        var handler = new DeleteUserCommandHandler(cache.Object, userRepository.Object, unitOfWork.Object);

        var result = await handler.Handle(new DeleteUserCommand(user.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        userRepository.Verify(r => r.Remove(user), Times.Once);
        cache.Verify(c => c.Remove(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task GetUserByIdQueryHandler_Should_Return_Cached_Value_When_Present()
    {
        var userId = Guid.NewGuid();
        var cached = new SeniorCrud.Application.DTOs.Users.UserResponseDto(userId, "User", "user@test.com", null, null, true, "User", []);

        var cache = new Mock<ICacheService>();
        cache.Setup(c => c.Get<SeniorCrud.Application.DTOs.Users.UserResponseDto>(It.IsAny<string>())).Returns(cached);

        var handler = new GetUserByIdQueryHandler(cache.Object, Mock.Of<IUserRepository>(), _mapper);

        var result = await handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(userId);
    }

    [Fact]
    public async Task GetUsersQueryHandler_Should_Return_List_And_Set_Cache_When_Miss()
    {
        var users = new List<User>
        {
            TestDataFactory.CreateUser(name: "Ana", email: "ana@test.com", cpf: "39053344705"),
            TestDataFactory.CreateUser(name: "Bruno", email: "bruno@test.com", cpf: "52998224725")
        };

        var cache = new Mock<ICacheService>();
        cache.Setup(c => c.Get<string>(It.IsAny<string>())).Returns((string?)null);
        cache.Setup(c => c.Get<IReadOnlyList<SeniorCrud.Application.DTOs.Users.UserListItemDto>>(It.IsAny<string>())).Returns((IReadOnlyList<SeniorCrud.Application.DTOs.Users.UserListItemDto>?)null);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetPagedAsync(1, 20, null, It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var handler = new GetUsersQueryHandler(cache.Object, repository.Object, _mapper);

        var result = await handler.Handle(new GetUsersQuery(1, 20, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        cache.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        cache.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<IReadOnlyList<SeniorCrud.Application.DTOs.Users.UserListItemDto>>(), It.IsAny<TimeSpan>()), Times.Once);
    }
}
