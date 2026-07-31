using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorCrud.Application.Features.Export.Commands;
using SeniorCrud.Application.DTOs.Users;
using SeniorCrud.Application.Features.Users.Commands;
using SeniorCrud.Application.Features.Users.Queries;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UsersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<IReadOnlyList<UserListItemDto>>), StatusCodes.Status200OK)]
    public async Task<Result<IReadOnlyList<UserListItemDto>>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetUsersQuery(pageNumber, pageSize, search), cancellationToken);
    }

    [HttpGet("export/csv")]
    [Produces("text/csv")]
    public async Task<ContentResult> ExportUsersCsv([FromQuery] List<Guid>? userIds, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ExportUsersCsvCommand(userIds), cancellationToken);

        if (!result.IsSuccess || string.IsNullOrEmpty(result.Value))
        {
            return new ContentResult
            {
                StatusCode = StatusCodes.Status404NotFound,
                ContentType = "text/plain",
                Content = result.Error.Description
            };
        }

        Response.Headers.ContentDisposition = "attachment; filename=users.csv";

        return new ContentResult
        {
            StatusCode = StatusCodes.Status200OK,
            ContentType = "text/csv",
            Content = result.Value
        };
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<UserResponseDto>> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<UserResponseDto>> CreateUser([FromBody] CreateUserDto request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateUserCommand>(request);
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<Result<UserResponseDto>> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateUserCommand>(request) with { UserId = id };
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<Result> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new DeleteUserCommand(id), cancellationToken);
    }
}
