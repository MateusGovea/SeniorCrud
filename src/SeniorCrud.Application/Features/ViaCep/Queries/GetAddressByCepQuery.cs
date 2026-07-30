using SeniorCrud.Application.Common.Messaging;
using SeniorCrud.Application.DTOs.ViaCep;

namespace SeniorCrud.Application.Features.ViaCep.Queries;

public sealed record GetAddressByCepQuery(string Cep) : IQuery<ViaCepResponseDto>;
