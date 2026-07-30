using SeniorCrud.Application.Common.Messaging;
using SeniorCrud.Application.DTOs.Addresses;

namespace SeniorCrud.Application.Features.Addresses.Commands;

public sealed record UpdateAddressCommand(
    Guid AddressId,
    string Cep,
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    bool IsPrimary) : ICommand<AddressResponseDto>;
