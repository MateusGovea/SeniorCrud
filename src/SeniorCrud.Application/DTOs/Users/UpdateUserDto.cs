namespace SeniorCrud.Application.DTOs.Users;

public sealed record UpdateUserDto(Guid UserId, string Nome, string Email, string? Cpf, DateOnly? BirthDate, bool IsActive);
