namespace PaymentGateway.Domain.Configuration;

public class CurrencyOptions
{
    public IReadOnlyCollection<string> Iso3CountryCodes { get; set; } = new List<string> { "GBP", "EUR", "USD" };
}