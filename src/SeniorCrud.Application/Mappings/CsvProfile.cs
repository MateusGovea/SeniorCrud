using AutoMapper;
using SeniorCrud.Application.DTOs.Export;
using SeniorCrud.Application.DTOs.Users;

namespace SeniorCrud.Application.Mappings;

public sealed class CsvProfile : Profile
{
    public CsvProfile()
    {
        CreateMap<UserListItemDto, ExportUsersCsvDto>()
            .ForCtorParam(nameof(ExportUsersCsvDto.Cpf), options => options.MapFrom(_ => (string?)null))
            .ForCtorParam(nameof(ExportUsersCsvDto.CreatedAt), options => options.MapFrom(_ => DateTimeOffset.MinValue));
    }
}
