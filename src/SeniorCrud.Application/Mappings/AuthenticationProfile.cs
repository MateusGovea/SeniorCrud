using AutoMapper;
using SeniorCrud.Application.DTOs.Authentication;
using SeniorCrud.Application.Features.Authentication.Commands;

namespace SeniorCrud.Application.Mappings;

public sealed class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<LoginRequestDto, LoginCommand>();
        CreateMap<LoginCommand, LoginRequestDto>();
    }
}
