using MediatR;

using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Application.Commands.Abstractions;

public record StoreProcessedPaymentResult(Guid Id);

public enum StoreProcessedPaymentCommandError
{
    UnexpectedException
}

public record StoreProcessedPaymentCommand(PaymentStatus Status, Payment Payment)
    : IRequest<Result<StoreProcessedPaymentResult, StoreProcessedPaymentCommandError>>;