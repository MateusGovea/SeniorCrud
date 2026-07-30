namespace SeniorCrud.Application.DTOs.Addresses;

public sealed record CreateAddressDto(
    Guid UserId,
    string Cep,
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    bool IsPrimary);
