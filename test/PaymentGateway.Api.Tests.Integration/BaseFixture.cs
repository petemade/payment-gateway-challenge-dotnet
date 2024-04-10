namespace PaymentGateway.Api.Tests.Integration;

[Collection("ApiUnderTest")]
public class BaseFixture : IClassFixture<TestApiWebApplicationFactory>
{
    protected TestApiWebApplicationFactory _factory;
    protected readonly HttpClient _api;

    protected BaseFixture(TestApiWebApplicationFactory factory)
    {
        _factory = factory;
        _api = factory.CreateDefaultClient();
    }
}