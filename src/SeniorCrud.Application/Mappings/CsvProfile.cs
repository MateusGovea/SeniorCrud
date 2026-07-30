using AutoMapper;
using SeniorCrud.Application.DTOs.Export;
using SeniorCrud.Domain.Entities;

namespace SeniorCrud.Application.Mappings;

public sealed class CsvProfile : Profile
{
    public CsvProfile()
    {
        CreateMap<User, ExportUsersCsvDto>()
            .ForCtorParam(nameof(ExportUsersCsvDto.Nome), options => options.MapFrom(source => source.Name))
            .ForCtorParam(nameof(ExportUsersCsvDto.Email), options => options.MapFrom(source => source.Email.Value))
            .ForCtorParam(nameof(ExportUsersCsvDto.Cpf), options => options.MapFrom(source => source.Cpf == null ? null : source.Cpf.Value))
            .ForCtorParam(nameof(ExportUsersCsvDto.Role), options => options.MapFrom(source => source.Role.ToString()))
            .ForCtorParam(nameof(ExportUsersCsvDto.CreatedAt), options => options.MapFrom(source => source.CreatedAt));
    }
}
