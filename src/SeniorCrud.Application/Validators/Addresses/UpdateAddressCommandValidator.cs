using FluentValidation;
using SeniorCrud.Application.Features.Addresses.Commands;
using SeniorCrud.Domain.Constants;

namespace SeniorCrud.Application.Validators.Addresses;

public sealed class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(command => command.AddressId)
            .NotEmpty();

        RuleFor(command => command.Cep)
            .NotEmpty()
            .Must(value => value.Where(char.IsDigit).Count() == DomainConstraints.CepLength)
            .WithMessage("Cep must contain 8 digits.");

        RuleFor(command => command.Street)
            .NotEmpty()
            .MaximumLength(DomainConstraints.MaxStreetLength);

        RuleFor(command => command.Number)
            .NotEmpty()
            .MaximumLength(DomainConstraints.MaxAddressNumberLength);

        RuleFor(command => command.Complement)
            .MaximumLength(DomainConstraints.MaxComplementLength)
            .When(command => !string.IsNullOrWhiteSpace(command.Complement));

        RuleFor(command => command.Neighborhood)
            .NotEmpty()
            .MaximumLength(DomainConstraints.MaxNeighborhoodLength);

        RuleFor(command => command.City)
            .NotEmpty()
            .MaximumLength(DomainConstraints.MaxCityLength);

        RuleFor(command => command.State)
            .NotEmpty()
            .Length(DomainConstraints.StateLength)
            .Matches("^[A-Za-z]+$");
    }
}
