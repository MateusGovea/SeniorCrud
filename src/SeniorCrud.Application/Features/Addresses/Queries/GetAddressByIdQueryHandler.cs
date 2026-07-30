using AutoMapper;
using MediatR;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Addresses.Queries;

public sealed class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, Result<AddressResponseDto>>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;

    public GetAddressByIdQueryHandler(IAddressRepository addressRepository, IMapper mapper)
    {
        _addressRepository = addressRepository;
        _mapper = mapper;
    }

    public async Task<Result<AddressResponseDto>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);
        if (address is null)
        {
            return Result<AddressResponseDto>.NotFound("Addresses.NotFound", "Address was not found.");
        }

        return Result<AddressResponseDto>.Success(_mapper.Map<AddressResponseDto>(address));
    }
}
