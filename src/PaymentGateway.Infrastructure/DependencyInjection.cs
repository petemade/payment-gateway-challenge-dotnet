using MediatR;

using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Application.Commands.Abstractions;
using PaymentGateway.Application.Queries.Abstractions;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Infrastructure.Commands.BankProcessCardPayment;
using PaymentGateway.Infrastructure.Commands.StoreProcessedPayment;
using PaymentGateway.Infrastructure.Queries.RetrieveProcessedPaymentFromStorage;

namespace PaymentGateway.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddSingleton<IRequestHandler<StoreProcessedPaymentCommand, Result<StoreProcessedPaymentResult, StoreProcessedPaymentCommandError>>,
                StoreProcessedPaymentCommandHandler>();

        services
            .AddSingleton<IRequestHandler<RetrieveProcessedPaymentFromStorageQuery, Payment?>,
                RetrieveProcessedPaymentFromStorageQueryHandler>();

        services
            .AddSingleton<IRequestHandler<BankProcessCardPaymentCommand, Result<BankPaymentProcessorResult, BankPaymentProcessorError>>,
                BankProcessCardPaymentCommandHandler>();

        return services;
    }
}