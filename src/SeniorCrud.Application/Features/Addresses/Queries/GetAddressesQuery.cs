using SeniorCrud.Application.Common.Messaging;
using SeniorCrud.Application.DTOs.Addresses;

namespace SeniorCrud.Application.Features.Addresses.Queries;

public sealed record GetAddressesQuery(int PageNumber = 1, int PageSize = 20, string? Search = null)
    : IQuery<IReadOnlyList<AddressResponseDto>>;