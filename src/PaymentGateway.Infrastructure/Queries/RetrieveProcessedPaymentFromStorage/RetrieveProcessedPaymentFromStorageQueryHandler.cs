using MediatR;

using PaymentGateway.Application.Queries.Abstractions;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Infrastructure.Queries.RetrieveProcessedPaymentFromStorage;

public class RetrieveProcessedPaymentFromStorageQueryHandler : IRequestHandler<RetrieveProcessedPaymentFromStorageQuery, Payment?>
{
    public Task<Payment?> Handle(RetrieveProcessedPaymentFromStorageQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(PaymentStorageStub.Retrieve(request.Id));
    }
}