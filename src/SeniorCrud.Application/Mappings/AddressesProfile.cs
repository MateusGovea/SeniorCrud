using AutoMapper;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Features.Addresses.Commands;

namespace SeniorCrud.Application.Mappings;

public sealed class AddressesProfile : Profile
{
    public AddressesProfile()
    {
        CreateMap<CreateAddressDto, CreateAddressCommand>();
        CreateMap<UpdateAddressDto, UpdateAddressCommand>();
    }
}
