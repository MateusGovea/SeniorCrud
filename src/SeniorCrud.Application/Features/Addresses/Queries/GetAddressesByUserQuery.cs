using SeniorCrud.Application.Common.Messaging;
using SeniorCrud.Application.DTOs.Addresses;

namespace SeniorCrud.Application.Features.Addresses.Queries;

public sealed record GetAddressesByUserQuery(Guid UserId) : IQuery<IReadOnlyList<AddressResponseDto>>;
