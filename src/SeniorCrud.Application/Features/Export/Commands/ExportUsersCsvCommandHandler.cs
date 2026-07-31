using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Csv;
using SeniorCrud.Application.DTOs.Export;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Export.Commands;

public sealed class ExportUsersCsvCommandHandler : IRequestHandler<ExportUsersCsvCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICsvExportService _csvExportService;
    private readonly IMapper _mapper;

    public ExportUsersCsvCommandHandler(
        IUserRepository userRepository,
        ICsvExportService csvExportService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _csvExportService = csvExportService;
        _mapper = mapper;
    }

    public Task<Result<string>> Handle(ExportUsersCsvCommand request, CancellationToken cancellationToken)
    {
        var query = _userRepository.Query();

        if (request.UserIds is { Count: > 0 })
        {
            var filter = request.UserIds.Distinct().ToArray();
            query = query.Where(user => filter.Contains(user.Id));
        }

        var users = query
            .OrderBy(user => user.Name)
            .ToList();

        var baseRows = _mapper.Map<List<ExportUsersCsvDto>>(users);
        var exportRows = new List<ExportUsersCsvDto>(users.Count);

        for (var i = 0; i < users.Count; i++)
        {
            var user = users[i];
            var baseRow = baseRows[i];
            var addresses = user.Addresses.ToList();

            if (addresses.Count == 0)
            {
                exportRows.Add(baseRow);
            }
            else
            {
                foreach (var address in addresses)
                {
                    exportRows.Add(baseRow with
                    {
                        Cep = address.Cep.Value,
                        Street = address.Street,
                        Number = address.Number.Value,
                        Complement = address.Complement,
                        Neighborhood = address.Neighborhood,
                        City = address.City,
                        State = address.State,
                        IsPrimary = address.IsPrimary,
                    });
                }
            }
        }

        var csvContent = _csvExportService.Export(exportRows);

        return Task.FromResult(Result<string>.Success(csvContent));
    }
}
