namespace SeniorCrud.Application.DTOs.Authentication;

public sealed record LoginResponseDto(Guid UserId, string Nome, string AccessToken, DateTimeOffset ExpiresAtUtc);
