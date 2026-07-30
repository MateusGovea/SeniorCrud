namespace SeniorCrud.Application.DTOs.Users;

public sealed record CreateUserDto(string Nome, string Email, string Password, string? Cpf, DateOnly? BirthDate);
