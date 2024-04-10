using FluentValidation;

using Microsoft.Extensions.Options;

using PaymentGateway.Domain.Configuration;

namespace PaymentGateway.Application.Commands.ProcessCardPayment;

public class ProcessCardPaymentCommandValidator : AbstractValidator<ProcessCardPaymentCommand>
{
    public ProcessCardPaymentCommandValidator(CurrencyOptions currencyOptions)
    {
        RuleFor(i => i.CardNumber)
            // Required
            .NotNull().NotEmpty()
            // Between 14-19 characters long
            .MinimumLength(14).MaximumLength(19)
            // Must only contain numeric characters
            .Must(i => i?.All(char.IsDigit) ?? false);

        // Required
        // 1-12
        RuleFor(i => i.ExpiryMonth).GreaterThanOrEqualTo(1).LessThanOrEqualTo(12);

        // Required
        RuleFor(i => i.ExpiryYear).GreaterThanOrEqualTo(DateTimeOffset.Now.Year);
            
        // Month + Year must be in the future
        RuleFor(i => new { i.ExpiryMonth, i.ExpiryYear })
            .Must(i => CardExpiryIsFutureDate(i.ExpiryMonth, i.ExpiryYear));

        RuleFor(i => i.Currency)
            // Required
            .NotNull().NotEmpty()
            // 3 characters
            .Length(3)
            // ISO country code https://www.xe.com/iso4217.php
            .Must(i => currencyOptions.Iso3CountryCodes.Contains(i));
        
        // Required
        // 0.01 = 1
        // £10.50 would be 1050
        RuleFor(i => i.Amount).GreaterThan(0);
        
        RuleFor(i => i.Cvv)
            // Required
            .NotNull().NotEmpty()
            // 3-4 characters long
            .MinimumLength(3).MaximumLength(4)
            // Numeric only
            .Must(i => i?.All(char.IsDigit) ?? false);
    }

    private static bool CardExpiryIsFutureDate(int expiryMonth, int expiryYear)
    {
        var cardExpiryDate = new DateTime(expiryYear, expiryMonth, DateTime.DaysInMonth(expiryYear, expiryMonth));

        var currentDate = DateTime.Now;
        var currentDateEndOfMonth = new DateTime(currentDate.Year, currentDate.Month,
            DateTime.DaysInMonth(currentDate.Year, currentDate.Month));

        return cardExpiryDate.Date > currentDateEndOfMonth.Date;
    }
}