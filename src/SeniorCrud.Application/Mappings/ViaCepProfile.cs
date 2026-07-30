using AutoMapper;
using SeniorCrud.Application.Abstractions.ViaCep;
using SeniorCrud.Application.DTOs.ViaCep;

namespace SeniorCrud.Application.Mappings;

public sealed class ViaCepProfile : Profile
{
    public ViaCepProfile()
    {
        CreateMap<ViaCepAddressResponse, ViaCepResponseDto>();
    }
}
