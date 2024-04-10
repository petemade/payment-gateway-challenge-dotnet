using MediatR;

using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Queries.Abstractions;

public record RetrieveProcessedPaymentFromStorageQuery(Guid Id) : IRequest<Payment?>;