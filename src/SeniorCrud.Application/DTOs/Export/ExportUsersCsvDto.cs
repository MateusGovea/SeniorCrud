namespace SeniorCrud.Application.DTOs.Export;

public sealed record ExportUsersCsvDto(
    Guid Id,
    string Nome,
    string Email,
    string? Cpf,
    string Role,
    bool IsActive,
    DateTimeOffset CreatedAt,
    string? Cep,
    string? Street,
    string? Number,
    string? Complement,
    string? Neighborhood,
    string? City,
    string? State,
    bool? IsPrimary);
