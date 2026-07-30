using SeniorCrud.Application.Common.Messaging;

namespace SeniorCrud.Application.Features.Addresses.Commands;

public sealed record DeleteAddressCommand(Guid AddressId) : ICommand;
