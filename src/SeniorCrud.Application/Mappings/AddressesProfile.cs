using AutoMapper;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Features.Addresses.Commands;
using SeniorCrud.Domain.Entities;

namespace SeniorCrud.Application.Mappings;

public sealed class AddressesProfile : Profile
{
    public AddressesProfile()
    {
        CreateMap<CreateAddressDto, CreateAddressCommand>();
        CreateMap<UpdateAddressDto, UpdateAddressCommand>();

        CreateMap<Address, AddressResponseDto>()
            .ForCtorParam(nameof(AddressResponseDto.Cep), options => options.MapFrom(source => source.Cep.Value))
            .ForCtorParam(nameof(AddressResponseDto.Number), options => options.MapFrom(source => source.Number.Value));
    }
}
