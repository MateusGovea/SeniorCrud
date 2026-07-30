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
            .ForCtorParam(nameof(ExportUsersCsvDto.CreatedAt), options => options.MapFrom(source => source.CreatedAt))
            .ForCtorParam(nameof(ExportUsersCsvDto.Cep), options => options.MapFrom(source =>
                source.Addresses.OrderByDescending(a => a.IsPrimary).ThenBy(a => a.CreatedAt).Select(a => a.Cep.Value).FirstOrDefault()))
            .ForCtorParam(nameof(ExportUsersCsvDto.Street), options => options.MapFrom(source =>
                source.Addresses.OrderByDescending(a => a.IsPrimary).ThenBy(a => a.CreatedAt).Select(a => a.Street).FirstOrDefault()))
            .ForCtorParam(nameof(ExportUsersCsvDto.Number), options => options.MapFrom(source =>
                source.Addresses.OrderByDescending(a => a.IsPrimary).ThenBy(a => a.CreatedAt).Select(a => a.Number.Value).FirstOrDefault()))
            .ForCtorParam(nameof(ExportUsersCsvDto.Complement), options => options.MapFrom(source =>
                source.Addresses.OrderByDescending(a => a.IsPrimary).ThenBy(a => a.CreatedAt).Select(a => a.Complement).FirstOrDefault()))
            .ForCtorParam(nameof(ExportUsersCsvDto.Neighborhood), options => options.MapFrom(source =>
                source.Addresses.OrderByDescending(a => a.IsPrimary).ThenBy(a => a.CreatedAt).Select(a => a.Neighborhood).FirstOrDefault()))
            .ForCtorParam(nameof(ExportUsersCsvDto.City), options => options.MapFrom(source =>
                source.Addresses.OrderByDescending(a => a.IsPrimary).ThenBy(a => a.CreatedAt).Select(a => a.City).FirstOrDefault()))
            .ForCtorParam(nameof(ExportUsersCsvDto.State), options => options.MapFrom(source =>
                source.Addresses.OrderByDescending(a => a.IsPrimary).ThenBy(a => a.CreatedAt).Select(a => a.State).FirstOrDefault()))
            .ForCtorParam(nameof(ExportUsersCsvDto.IsPrimary), options => options.MapFrom(source =>
                (bool?)source.Addresses.OrderByDescending(a => a.IsPrimary).ThenBy(a => a.CreatedAt).Select(a => (bool?)a.IsPrimary).FirstOrDefault()));
    }
}
