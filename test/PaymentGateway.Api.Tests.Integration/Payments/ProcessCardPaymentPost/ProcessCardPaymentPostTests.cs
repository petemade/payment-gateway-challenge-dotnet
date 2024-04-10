using System.Net;
using System.Net.Http.Json;

using AutoFixture;

using FluentAssertions;
using FluentAssertions.Execution;

using PaymentGateway.Application.Commands.ProcessCardPayment;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Api.Tests.Integration.Payments.ProcessCardPaymentPost;

public class ProcessCardPaymentPostTests : BaseFixture
{
    private readonly IFixture _fixture;

    public ProcessCardPaymentPostTests(TestApiWebApplicationFactory factory)
        : base(factory)
    {
        _fixture = new Fixture();
    }

    // TODO add mock bank processing service
    // Currently will only work when running the bank mock
    [Theory]
    [InlineData("2222405343248877", "8877", 4, 2025, "GBP", "123", 100, HttpStatusCode.OK)]
    [InlineData("2222405343248112", "8112", 1, 2026, "USD", "456", 60000, HttpStatusCode.PaymentRequired)]
    public async Task GivenValidRequest_Success_IsReturned(
        string cardNumber, string lastFourDigits, int month, int year, string currency, string cvv, int amount, HttpStatusCode statusCodeResponse)
    {
        var paymentRequest = _fixture.Build<ProcessCardPaymentCommand>()
            .With(i => i.CardNumber, cardNumber)
            .With(i => i.ExpiryMonth, month)
            .With(i => i.ExpiryYear, year)
            .With(i => i.Currency, currency)
            .With(i => i.Amount, amount)
            .With(i => i.Cvv, cvv)
            .Create();

        var response = await _api.PostAsJsonAsync("/payments/card", paymentRequest);
        response.StatusCode.Should().Be(statusCodeResponse);

        if (statusCodeResponse != HttpStatusCode.OK) return;

        var responseObject = await response.Content.ReadFromJsonAsync<ProcessCardPaymentCommandResult>();

        using (new AssertionScope())
        {
            responseObject.Should().NotBeNull();
            responseObject!.Status.Should().Be(PaymentStatus.Authorized);
            responseObject.Id.Should().NotBeEmpty();
            responseObject.ExpiryMonth.Should().Be(month);
            responseObject.ExpiryYear.Should().Be(year);
            responseObject.Amount.Should().Be(amount);
            responseObject.Currency.Should().Be(currency);
            responseObject.LastFourDigits.Should().Be(lastFourDigits);
        }
    }

    // TODO endpoint
    [Fact]
    public async Task GivenValidRequest_Success_AndId_AreReturned_And_Payment_IsRetrieved()
    {
        var paymentRequest = _fixture.Build<ProcessCardPaymentCommand>()
            .With(i => i.CardNumber, "2222405343248877")
            .With(i => i.ExpiryMonth, 4)
            .With(i => i.ExpiryYear, 2025)
            .With(i => i.Currency, "GBP")
            .With(i => i.Amount, 100)
            .With(i => i.Cvv, "123")
            .Create();

        var postResponse = await _api.PostAsJsonAsync("/payments/card", paymentRequest);
        var responseObject = await postResponse.Content.ReadFromJsonAsync<ProcessCardPaymentCommandResult>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _api.GetAsync($"/payments/card/{responseObject!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}