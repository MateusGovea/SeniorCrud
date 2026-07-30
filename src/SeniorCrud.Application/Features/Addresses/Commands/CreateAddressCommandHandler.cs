using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Common.Caching;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;
using SeniorCrud.Domain.Entities;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.Application.Features.Addresses.Commands;

public sealed class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Result<AddressResponseDto>>
{
    private readonly IAddressRepository _addressRepository;
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateAddressCommandHandler(
        IAddressRepository addressRepository,
        ICacheService cacheService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _addressRepository = addressRepository;
        _cacheService = cacheService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AddressResponseDto>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<AddressResponseDto>.NotFound("Users.NotFound", "User was not found.");
        }

        var now = DateTimeOffset.UtcNow;
        var address = new Address(
            id: Guid.NewGuid(),
            userId: request.UserId,
            cep: new Cep(request.Cep),
            street: request.Street,
            number: new AddressNumber(request.Number),
            complement: request.Complement,
            neighborhood: request.Neighborhood,
            city: request.City,
            state: request.State,
            isPrimary: request.IsPrimary,
            createdAtUtc: now);

        await _addressRepository.AddAsync(address, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cacheService.Remove(ApplicationCacheKeys.Address(address.Id));
        _cacheService.Remove(ApplicationCacheKeys.UserAddresses(address.UserId));

        return Result<AddressResponseDto>.Success(_mapper.Map<AddressResponseDto>(address));
    }
}
