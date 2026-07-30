using AutoMapper;
using MediatR;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Addresses.Queries;

public sealed class GetAddressesByUserQueryHandler : IRequestHandler<GetAddressesByUserQuery, Result<IReadOnlyList<AddressResponseDto>>>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAddressesByUserQueryHandler(
        IAddressRepository addressRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _addressRepository = addressRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<AddressResponseDto>>> Handle(GetAddressesByUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<IReadOnlyList<AddressResponseDto>>.NotFound("Users.NotFound", "User was not found.");
        }

        var addresses = await _addressRepository.ListByUserIdAsync(request.UserId, cancellationToken);
        var response = _mapper.Map<IReadOnlyList<AddressResponseDto>>(addresses);

        return Result<IReadOnlyList<AddressResponseDto>>.Success(response);
    }
}
