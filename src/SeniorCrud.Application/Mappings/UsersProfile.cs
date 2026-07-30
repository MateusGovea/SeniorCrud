using AutoMapper;
using SeniorCrud.Application.DTOs.Users;
using SeniorCrud.Application.Features.Users.Commands;

namespace SeniorCrud.Application.Mappings;

public sealed class UsersProfile : Profile
{
    public UsersProfile()
    {
        CreateMap<CreateUserDto, CreateUserCommand>();
        CreateMap<UpdateUserDto, UpdateUserCommand>();
    }
}
