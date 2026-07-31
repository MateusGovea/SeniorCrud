using Asp.Versioning;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Features.Addresses.Commands;
using SeniorCrud.Application.Features.Addresses.Queries;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/addresses")]
[Authorize]
public sealed class AddressesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AddressesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<IReadOnlyList<AddressResponseDto>>), StatusCodes.Status200OK)]
    public async Task<Result<IReadOnlyList<AddressResponseDto>>> GetAddresses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetAddressesQuery(pageNumber, pageSize, search), cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<AddressResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<AddressResponseDto>> GetAddressById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetAddressByIdQuery(id), cancellationToken);
    }

    [HttpGet("~/api/v{version:apiVersion}/users/{userId:guid}/addresses")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<AddressResponseDto>>), StatusCodes.Status200OK)]
    public async Task<Result<IReadOnlyList<AddressResponseDto>>> GetAddressesByUser([FromRoute] Guid userId, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetAddressesByUserQuery(userId), cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<AddressResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<AddressResponseDto>> CreateAddress([FromBody] CreateAddressDto request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateAddressCommand>(request);
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<AddressResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<AddressResponseDto>> UpdateAddress([FromRoute] Guid id, [FromBody] UpdateAddressDto request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateAddressCommand>(request) with { AddressId = id };
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<Result> DeleteAddress([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new DeleteAddressCommand(id), cancellationToken);
    }
}
