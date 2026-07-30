namespace SeniorCrud.Application.DTOs.Export;

public sealed record ExportUsersCsvDto(
    Guid Id,
    string Nome,
    string Email,
    string? Cpf,
    string Role,
    bool IsActive,
    DateTimeOffset CreatedAt);
