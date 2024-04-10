using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace PaymentGateway.Api.Tests.Integration;

public class TestApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Override services
        });

        builder.UseEnvironment("Development");
    }

    protected override void Dispose(bool disposing)
    {
    }
}