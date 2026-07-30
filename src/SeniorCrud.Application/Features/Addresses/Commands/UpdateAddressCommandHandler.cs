using AutoMapper;
using MediatR;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.Application.Features.Addresses.Commands;

public sealed class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Result<AddressResponseDto>>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateAddressCommandHandler(
        IAddressRepository addressRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AddressResponseDto>> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);
        if (address is null)
        {
            return Result<AddressResponseDto>.NotFound("Addresses.NotFound", "Address was not found.");
        }

        var now = DateTimeOffset.UtcNow;

        address.Update(
            cep: new Cep(request.Cep),
            street: request.Street,
            number: new AddressNumber(request.Number),
            complement: request.Complement,
            neighborhood: request.Neighborhood,
            city: request.City,
            state: request.State,
            updatedAtUtc: now);

        if (request.IsPrimary)
        {
            address.SetAsPrimary(now);
        }
        else
        {
            address.SetAsSecondary(now);
        }

        _addressRepository.Update(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AddressResponseDto>.Success(_mapper.Map<AddressResponseDto>(address));
    }
}
