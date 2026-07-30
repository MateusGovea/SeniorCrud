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

        var exportRows = _mapper.Map<IReadOnlyList<ExportUsersCsvDto>>(users);
        var csvContent = _csvExportService.Export(exportRows);

        return Task.FromResult(Result<string>.Success(csvContent));
    }
}
