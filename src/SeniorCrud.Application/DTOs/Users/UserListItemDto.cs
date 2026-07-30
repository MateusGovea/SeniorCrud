namespace SeniorCrud.Application.DTOs.Users;

public sealed record UserListItemDto(Guid Id, string Nome, string Email, bool IsActive, string Role);
