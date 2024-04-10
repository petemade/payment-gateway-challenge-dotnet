using AutoFixture;

using AutoMapper;

using FluentAssertions;
using FluentAssertions.Execution;

using PaymentGateway.Application.Commands.ProcessCardPayment;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Tests.Unit.Mapping;

public class ProcessCardPaymentCommandMappingProfileTests
{
    private readonly IMapper _mapper;
    private readonly IFixture _fixture;

    public ProcessCardPaymentCommandMappingProfileTests()
    {
        _mapper = new MapperConfiguration(config => config.AddProfile<ProcessCardPaymentCommandMappingProfile>())
            .CreateMapper();

        _fixture = new Fixture();
    }

    [Fact]
    public void MappingProfile_ShouldBe_Valid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void MappingProfile_ShouldMap_ProcessCardPaymentCommand_To_Payment()
    {
        var source = _fixture.Create<ProcessCardPaymentCommand>();

        var destination = _mapper.Map<Payment>(source);

        using (new AssertionScope())
        {
            destination.Should().NotBeNull();
            destination.Should().BeEquivalentTo(source);
        }
    }
}