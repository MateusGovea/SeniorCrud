using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Features.Addresses.Commands;
using SeniorCrud.Application.Features.Addresses.Queries;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Api.Controllers;

[ApiController]
public sealed class AddressesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AddressesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("api/addresses/{id:guid}")]
    [ProducesResponseType(typeof(Result<AddressResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<AddressResponseDto>> GetAddressById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetAddressByIdQuery(id), cancellationToken);
    }

    [HttpGet("api/users/{userId:guid}/addresses")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<AddressResponseDto>>), StatusCodes.Status200OK)]
    public async Task<Result<IReadOnlyList<AddressResponseDto>>> GetAddressesByUser([FromRoute] Guid userId, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetAddressesByUserQuery(userId), cancellationToken);
    }

    [HttpPost("api/addresses")]
    [ProducesResponseType(typeof(Result<AddressResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<AddressResponseDto>> CreateAddress([FromBody] CreateAddressDto request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateAddressCommand>(request);
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPut("api/addresses/{id:guid}")]
    [ProducesResponseType(typeof(Result<AddressResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<AddressResponseDto>> UpdateAddress([FromRoute] Guid id, [FromBody] UpdateAddressDto request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateAddressCommand>(request) with { AddressId = id };
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpDelete("api/addresses/{id:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<Result> DeleteAddress([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new DeleteAddressCommand(id), cancellationToken);
    }
}
