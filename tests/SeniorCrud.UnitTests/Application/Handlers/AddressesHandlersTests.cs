using AutoMapper;
using FluentAssertions;
using Moq;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Features.Addresses.Commands;
using SeniorCrud.Application.Features.Addresses.Queries;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Domain.Entities;
using SeniorCrud.UnitTests.Common;

namespace SeniorCrud.UnitTests.Application.Handlers;

public class AddressesHandlersTests
{
    private readonly IMapper _mapper = TestDataFactory.CreateMapper();

    [Fact]
    public async Task CreateAddressCommandHandler_Should_Return_NotFound_When_User_Missing()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var handler = new CreateAddressCommandHandler(
            Mock.Of<IAddressRepository>(),
            Mock.Of<ICacheService>(),
            userRepository.Object,
            Mock.Of<IUnitOfWork>(),
            _mapper);

        var result = await handler.Handle(
            new CreateAddressCommand(Guid.NewGuid(), "01001000", "Rua", "10", null, "Centro", "Sao Paulo", "SP", true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(SeniorCrud.Application.Results.ErrorType.NotFound);
    }

    [Fact]
    public async Task CreateAddressCommandHandler_Should_Create_Address_When_Valid()
    {
        var user = TestDataFactory.CreateUser();
        Address? createdAddress = null;

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var addressRepository = new Mock<IAddressRepository>();
        addressRepository
            .Setup(r => r.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()))
            .Callback<Address, CancellationToken>((address, _) => createdAddress = address)
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var cache = new Mock<ICacheService>();

        var handler = new CreateAddressCommandHandler(addressRepository.Object, cache.Object, userRepository.Object, unitOfWork.Object, _mapper);

        var result = await handler.Handle(
            new CreateAddressCommand(user.Id, "01001000", "Rua", "10", null, "Centro", "Sao Paulo", "SP", true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        createdAddress.Should().NotBeNull();
        cache.Verify(c => c.Remove(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task UpdateAddressCommandHandler_Should_Return_NotFound_When_Address_Missing()
    {
        var addressRepository = new Mock<IAddressRepository>();
        addressRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Address?)null);

        var handler = new UpdateAddressCommandHandler(addressRepository.Object, Mock.Of<ICacheService>(), Mock.Of<IUnitOfWork>(), _mapper);

        var result = await handler.Handle(
            new UpdateAddressCommand(Guid.NewGuid(), "01001000", "Rua", "10", null, "Centro", "Sao Paulo", "SP", true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(SeniorCrud.Application.Results.ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteAddressCommandHandler_Should_Remove_Address_When_Exists()
    {
        var address = TestDataFactory.CreateAddress();
        var addressRepository = new Mock<IAddressRepository>();
        addressRepository.Setup(r => r.GetByIdAsync(address.Id, It.IsAny<CancellationToken>())).ReturnsAsync(address);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var cache = new Mock<ICacheService>();

        var handler = new DeleteAddressCommandHandler(addressRepository.Object, cache.Object, unitOfWork.Object);

        var result = await handler.Handle(new DeleteAddressCommand(address.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        addressRepository.Verify(r => r.Remove(address), Times.Once);
    }

    [Fact]
    public async Task GetAddressByIdQueryHandler_Should_Return_Cached_When_Available()
    {
        var addressId = Guid.NewGuid();
        var cached = new SeniorCrud.Application.DTOs.Addresses.AddressResponseDto(
            addressId,
            Guid.NewGuid(),
            "01001000",
            "Rua",
            "10",
            null,
            "Centro",
            "Sao Paulo",
            "SP",
            true);

        var cache = new Mock<ICacheService>();
        cache.Setup(c => c.Get<SeniorCrud.Application.DTOs.Addresses.AddressResponseDto>(It.IsAny<string>())).Returns(cached);

        var handler = new GetAddressByIdQueryHandler(cache.Object, Mock.Of<IAddressRepository>(), _mapper);

        var result = await handler.Handle(new GetAddressByIdQuery(addressId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(addressId);
    }

    [Fact]
    public async Task GetAddressesByUserQueryHandler_Should_Return_List_When_User_Exists()
    {
        var user = TestDataFactory.CreateUser();
        var addresses = new List<Address>
        {
            TestDataFactory.CreateAddress(userId: user.Id, isPrimary: true),
            TestDataFactory.CreateAddress(userId: user.Id, isPrimary: false)
        };

        var cache = new Mock<ICacheService>();
        cache.Setup(c => c.Get<IReadOnlyList<SeniorCrud.Application.DTOs.Addresses.AddressResponseDto>>(It.IsAny<string>())).Returns((IReadOnlyList<SeniorCrud.Application.DTOs.Addresses.AddressResponseDto>?)null);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var addressRepository = new Mock<IAddressRepository>();
        addressRepository.Setup(r => r.ListByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(addresses);

        var handler = new GetAddressesByUserQueryHandler(cache.Object, addressRepository.Object, userRepository.Object, _mapper);

        var result = await handler.Handle(new GetAddressesByUserQuery(user.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        cache.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<IReadOnlyList<SeniorCrud.Application.DTOs.Addresses.AddressResponseDto>>(), It.IsAny<TimeSpan>()), Times.Once);
    }
}
