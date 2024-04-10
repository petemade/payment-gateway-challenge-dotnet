using FluentValidation;

using PaymentGateway.Api.Extensions;
using PaymentGateway.Application.Commands.ProcessCardPayment;
using PaymentGateway.Domain.Configuration;
using PaymentGateway.Infrastructure;
using PaymentGateway.Infrastructure.Commands.BankProcessCardPayment;

var builder = WebApplication.CreateBuilder(args);

builder.AddTelemetry();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(PaymentGateway.Api.Program), typeof(ProcessCardPaymentCommandMappingProfile), 
    typeof(BankProcessCardPaymentMappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<ProcessCardPaymentCommandValidator>();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblyContaining<ProcessCardPaymentCommandHandler>());

builder.Services.AddInfrastructure();
builder.Services.AddSingleton<CurrencyOptions>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace PaymentGateway.Api
{
    public partial class Program { }
}