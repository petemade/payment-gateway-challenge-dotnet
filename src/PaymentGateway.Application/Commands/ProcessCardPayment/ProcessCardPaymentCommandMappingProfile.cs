using AutoMapper;

using PaymentGateway.Application.Extensions;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Commands.ProcessCardPayment;

public class ProcessCardPaymentCommandMappingProfile : Profile
{
    public ProcessCardPaymentCommandMappingProfile()
    {
        CreateMap<ProcessCardPaymentCommand, Payment>()
            .ForMemberMapFrom(dest => dest.CardNumber, src => src.CardNumber)
            .ForMemberMapFrom(dest => dest.ExpiryMonth, src => src.ExpiryMonth)
            .ForMemberMapFrom(dest => dest.ExpiryYear, src => src.ExpiryYear)
            .ForMemberMapFrom(dest => dest.Currency, src => src.Currency)
            .ForMemberMapFrom(dest => dest.Amount, src => src.Amount)
            .ForMemberMapFrom(dest => dest.Cvv, src => src.Cvv);
    }
}