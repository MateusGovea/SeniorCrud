using SeniorCrud.Application.DTOs.Addresses;

namespace SeniorCrud.Application.DTOs.Users;

public sealed record UserResponseDto(
    Guid Id,
    string Nome,
    string Email,
    string? Cpf,
    DateOnly? BirthDate,
    bool IsActive,
    string Role,
    IReadOnlyCollection<AddressResponseDto> Addresses);
