using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using PaymentGateway.Application.Commands.Abstractions;
using PaymentGateway.Domain;

namespace PaymentGateway.Infrastructure.Commands.BankProcessCardPayment;

internal class BankPaymentProcessorRequest
{
    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; } = null!;

    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; } = null!;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("cvv")]
    public string Cvv { get; set; } = null!;
}

internal class BankPaymentProcessorResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; } = null!;
}

public class BankProcessCardPaymentCommandHandler
    : IRequestHandler<BankProcessCardPaymentCommand, Result<BankPaymentProcessorResult, BankPaymentProcessorError>>
{
    private readonly HttpClient _client;
    private readonly IMapper _mapper;
    private readonly ILogger<BankProcessCardPaymentCommandHandler> _logger;

    public BankProcessCardPaymentCommandHandler(IHttpClientFactory httpClientFactory, IMapper mapper, ILogger<BankProcessCardPaymentCommandHandler> logger)
    {
        _client = httpClientFactory.CreateClient();
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<BankPaymentProcessorResult, BankPaymentProcessorError>> Handle(
        BankProcessCardPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bankProcessingRequest = _mapper.Map<BankPaymentProcessorRequest>(request);

            // TODO move endpoint to configuration
            // Polly against the http client
            var response = await _client.PostAsJsonAsync("http://localhost:8080/payments", bankProcessingRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<BankPaymentProcessorResponse>(CancellationToken.None);

                return _mapper.Map<BankPaymentProcessorResult>(responseData);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest) return BankPaymentProcessorError.BadRequest;

            _logger.LogError("Unexpected response received from bank processing, {status}", response.StatusCode);

            return BankPaymentProcessorError.UnexpectedResponse;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "An unexpected exception occurred");
            return BankPaymentProcessorError.UnexpectedException;
        }
    }
}