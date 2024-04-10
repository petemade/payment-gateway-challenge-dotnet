using MediatR;

using PaymentGateway.Application.Commands.Abstractions;
using PaymentGateway.Domain;

namespace PaymentGateway.Infrastructure.Commands.StoreProcessedPayment;

public class StoreProcessedPaymentCommandHandler
    : IRequestHandler<StoreProcessedPaymentCommand, Result<StoreProcessedPaymentResult, StoreProcessedPaymentCommandError>>
{
    public Task<Result<StoreProcessedPaymentResult, StoreProcessedPaymentCommandError>> Handle(
        StoreProcessedPaymentCommand request, CancellationToken cancellationToken)
    {
        var id = PaymentStorageStub.Store(request.Payment);

        Result<StoreProcessedPaymentResult, StoreProcessedPaymentCommandError> result = new StoreProcessedPaymentResult(id);

        return Task.FromResult(result);
    }
}