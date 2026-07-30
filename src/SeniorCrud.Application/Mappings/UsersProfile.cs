using AutoMapper;
using SeniorCrud.Application.DTOs.Users;
using SeniorCrud.Application.Features.Users.Commands;
using SeniorCrud.Domain.Entities;

namespace SeniorCrud.Application.Mappings;

public sealed class UsersProfile : Profile
{
    public UsersProfile()
    {
        CreateMap<CreateUserDto, CreateUserCommand>();
        CreateMap<UpdateUserDto, UpdateUserCommand>();

        CreateMap<User, UserResponseDto>()
            .ForCtorParam(nameof(UserResponseDto.Nome), options => options.MapFrom(source => source.Name))
            .ForCtorParam(nameof(UserResponseDto.Email), options => options.MapFrom(source => source.Email.Value))
            .ForCtorParam(nameof(UserResponseDto.Cpf), options => options.MapFrom(source => source.Cpf == null ? null : source.Cpf.Value))
            .ForCtorParam(nameof(UserResponseDto.Role), options => options.MapFrom(source => source.Role.ToString()));

        CreateMap<User, UserListItemDto>()
            .ForCtorParam(nameof(UserListItemDto.Nome), options => options.MapFrom(source => source.Name))
            .ForCtorParam(nameof(UserListItemDto.Email), options => options.MapFrom(source => source.Email.Value))
            .ForCtorParam(nameof(UserListItemDto.Role), options => options.MapFrom(source => source.Role.ToString()));
    }
}
