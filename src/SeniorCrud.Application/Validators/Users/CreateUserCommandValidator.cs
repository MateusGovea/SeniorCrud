using FluentValidation;
using SeniorCrud.Application.Features.Users.Commands;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Enums;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.Application.Validators.Users;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(command => command.Nome)
            .NotEmpty()
            .MinimumLength(DomainConstraints.MinUserNameLength)
            .MaximumLength(DomainConstraints.MaxUserNameLength);

        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(DomainConstraints.MaxEmailLength)
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(200);

        RuleFor(command => command.Cpf)
            .Must(value => string.IsNullOrWhiteSpace(value) || Cpf.IsValid(value))
            .WithMessage("Informe um CPF válido.");

        RuleFor(command => command.Role)
            .NotEmpty()
            .IsEnumName(typeof(UserRole));

        RuleFor(command => command.BirthDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(command => command.BirthDate.HasValue);
    }
}
