using SeniorCrud.Application.Common.Messaging;

namespace SeniorCrud.Application.Features.Export.Queries;

public sealed record ExportUsersCsvQuery(IReadOnlyCollection<Guid>? UserIds = null) : IQuery<string>;
