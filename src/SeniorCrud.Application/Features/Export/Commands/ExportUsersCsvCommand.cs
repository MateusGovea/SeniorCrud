using SeniorCrud.Application.Common.Messaging;

namespace SeniorCrud.Application.Features.Export.Commands;

public sealed record ExportUsersCsvCommand(IReadOnlyCollection<Guid>? UserIds = null) : ICommand<string>;
