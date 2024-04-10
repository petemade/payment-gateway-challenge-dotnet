using AutoMapper;

using PaymentGateway.Application.Commands.Abstractions;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Infrastructure.Commands.BankProcessCardPayment;

public class BankProcessCardPaymentMappingProfile : Profile
{
    public BankProcessCardPaymentMappingProfile()
    {
        CreateMap<BankProcessCardPaymentCommand, BankPaymentProcessorRequest>()
            .ForMemberMapFrom(dest => dest.CardNumber, src => src.Payment.CardNumber)
            .ForMemberMapFrom(dest => dest.ExpiryDate, src => $"{src.Payment.ExpiryMonth:00}/{src.Payment.ExpiryYear}")
            .ForMemberMapFrom(dest => dest.Currency, src => src.Payment.Currency)
            .ForMemberMapFrom(dest => dest.Amount, src => src.Payment.Amount)
            .ForMemberMapFrom(dest => dest.Cvv, src => src.Payment.Cvv);

        CreateMap<BankPaymentProcessorResponse, BankPaymentProcessorResult>()
            .ForMemberMapFrom(dest => dest.Status,
                src => src.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined)
            .ForMemberMapFrom(dest => dest.AuthorizationCode, src => src.AuthorizationCode);
    }
}