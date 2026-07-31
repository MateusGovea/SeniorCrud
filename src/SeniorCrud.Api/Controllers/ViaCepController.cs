using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorCrud.Application.DTOs.ViaCep;
using SeniorCrud.Application.Features.ViaCep.Queries;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/viacep")]
[Authorize]
public sealed class ViaCepController : ControllerBase
{
    private readonly IMediator _mediator;

    public ViaCepController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{cep}")]
    [ProducesResponseType(typeof(Result<ViaCepResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<ViaCepResponseDto>> GetByCep([FromRoute] string cep, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetAddressByCepQuery(cep), cancellationToken);
    }
}
