using MediatR;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Addresses.Commands;

public sealed class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Result>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAddressCommandHandler(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);
        if (address is null)
        {
            return Result.NotFound("Addresses.NotFound", "Address was not found.");
        }

        _addressRepository.Remove(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
