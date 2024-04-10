using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using PaymentGateway.Application.Commands.Abstractions;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Application.Commands.ProcessCardPayment;

public record ProcessCardPaymentCommandResult(Guid Id, PaymentStatus Status, string LastFourDigits, int ExpiryMonth,
    int ExpiryYear, string Currency, int Amount);

public enum ProcessCardPaymentCommandError
{
    InvalidRequest,
    UnexpectedException
}

public record ProcessCardPaymentCommand
    : IRequest<Result<ProcessCardPaymentCommandResult, ProcessCardPaymentCommandError>>
{
    public string? CardNumber { get; set; }

    public int ExpiryMonth { get; set; }

    public int ExpiryYear { get; set; }

    public string? Currency { get; set; }

    public int Amount { get; set; }

    public string? Cvv { get; set; }
}

public class ProcessCardPaymentCommandHandler
    : IRequestHandler<ProcessCardPaymentCommand, Result<ProcessCardPaymentCommandResult, ProcessCardPaymentCommandError>>
{
    private readonly ILogger<ProcessCardPaymentCommandHandler> _logger;
    private readonly IValidator<ProcessCardPaymentCommand> _commandValidator;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public ProcessCardPaymentCommandHandler(
        ILogger<ProcessCardPaymentCommandHandler> logger, IValidator<ProcessCardPaymentCommand> commandValidator, IMapper mapper,
        IMediator mediator)
    {
        _logger = logger;
        _commandValidator = commandValidator;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<Result<ProcessCardPaymentCommandResult, ProcessCardPaymentCommandError>> Handle(
        ProcessCardPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _commandValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid) return ProcessCardPaymentCommandError.InvalidRequest;

            var paymentEntity = _mapper.Map<Payment>(request);
            var bankProcessingRequest = new BankProcessCardPaymentCommand(paymentEntity);
            var bankProcessingResponse = await _mediator.Send(bankProcessingRequest, cancellationToken);

            if (!bankProcessingResponse.IsSuccess)
            {
                // TODO use mapper
                return bankProcessingResponse.Error switch
                {
                    BankPaymentProcessorError.BadRequest => ProcessCardPaymentCommandError.InvalidRequest,
                    BankPaymentProcessorError.UnexpectedException => ProcessCardPaymentCommandError.UnexpectedException,
                    _ => ProcessCardPaymentCommandError.UnexpectedException
                };
            }

            // TODO store authorization code
            var storageRequest = new StoreProcessedPaymentCommand(bankProcessingResponse.Value.Status, paymentEntity);
            var storageResult = await _mediator.Send(storageRequest, CancellationToken.None);

            if (!storageResult.IsSuccess)
            {
                // TODO use mapper
                return storageResult.Error switch
                {
                    StoreProcessedPaymentCommandError.UnexpectedException => ProcessCardPaymentCommandError.UnexpectedException,
                    _ => ProcessCardPaymentCommandError.UnexpectedException
                };
            }

            var lastFourDigits = paymentEntity.CardNumber[^4..];

            return new ProcessCardPaymentCommandResult(storageResult.Value.Id, bankProcessingResponse.Value.Status,
                lastFourDigits, paymentEntity.ExpiryMonth, paymentEntity.ExpiryYear, paymentEntity.Currency,
                paymentEntity.Amount);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "An unexpected exception occurred");
            return ProcessCardPaymentCommandError.UnexpectedException;
        }
    }
}