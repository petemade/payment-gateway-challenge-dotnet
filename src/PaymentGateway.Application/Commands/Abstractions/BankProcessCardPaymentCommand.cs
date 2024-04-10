using MediatR;

using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Application.Commands.Abstractions;

public class BankPaymentProcessorResult
{
    public PaymentStatus Status { get; set; }

    public string AuthorizationCode { get; set; } = null!;
}

public enum BankPaymentProcessorError
{
    BadRequest,
    UnexpectedResponse,
    UnexpectedException
}

public record BankProcessCardPaymentCommand
    (Payment Payment) : IRequest<Result<BankPaymentProcessorResult, BankPaymentProcessorError>>;