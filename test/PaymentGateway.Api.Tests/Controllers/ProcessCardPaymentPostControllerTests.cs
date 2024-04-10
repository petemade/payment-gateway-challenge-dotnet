using AutoFixture;

using FluentAssertions;
using FluentAssertions.Execution;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Moq;

using PaymentGateway.Api.Controllers.Payments;
using PaymentGateway.Application.Commands.ProcessCardPayment;

namespace PaymentGateway.Api.Tests.Unit.Controllers;

public class ProcessCardPaymentPostControllerTests
{
    private readonly IFixture _fixture;

    public ProcessCardPaymentPostControllerTests()
    {
        _fixture = new Fixture();
    }

    #region London (behaviour)

    [Fact]
    public async Task GivenValidInput_MediatorIsCalledOnce()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(i => i.Send(It.IsAny<ProcessCardPaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_fixture.Create<ProcessCardPaymentCommandResult>());

        var controller = new ProcessCardPaymentPostController(mediator.Object);

        // Act
        await controller.Post(new(), CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            mediator.Verify(i => i.Send(It.IsAny<ProcessCardPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.VerifyNoOtherCalls();
        }
    }

    #endregion

    #region Chicago (output)

    [Theory]
    [InlineData(ProcessCardPaymentCommandError.InvalidRequest, 400)]
    [InlineData(ProcessCardPaymentCommandError.UnexpectedException, 500)]
    public async Task GivenValidInput_MediatorReturnsAnError_CorrectStatusCode_IsReturned(
        ProcessCardPaymentCommandError error, int statusCode)
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(i => i.Send(It.IsAny<ProcessCardPaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(error);

        var controller = new ProcessCardPaymentPostController(mediator.Object);

        // Act
        var response = await controller.Post(new(), CancellationToken.None) as ObjectResult;

        // Assert
        using (new AssertionScope())
        {
            response!.StatusCode.Should().Be(statusCode);
        }
    }

    #endregion
}