using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SeniorCrud.Application.DTOs.Authentication;
using SeniorCrud.Application.Features.Authentication.Commands;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AuthController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<LoginResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<LoginResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<LoginCommand>(request);
        return await _mediator.Send(command, cancellationToken);
    }
}
