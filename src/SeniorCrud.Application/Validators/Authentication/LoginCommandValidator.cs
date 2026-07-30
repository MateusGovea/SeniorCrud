using FluentValidation;
using SeniorCrud.Application.Features.Authentication.Commands;

namespace SeniorCrud.Application.Validators.Authentication;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(255)
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(200);
    }
}
