using System.Net;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Application.Commands.ProcessCardPayment;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Api.Controllers.Payments;

[Route("/payments/card")]
public class ProcessCardPaymentPostController : Controller
{
    private readonly IMediator _mediator;

    public ProcessCardPaymentPostController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Processes a card payment post request.
    /// </summary>
    /// <param name="command">The command containing the payment and card details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A payment Id, status and payment details, or an HTTP status response.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProcessCardPaymentCommandResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.PaymentRequired)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Post([FromBody] ProcessCardPaymentCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                ProcessCardPaymentCommandError.InvalidRequest => BadRequest("The payment request was invalid"),
                ProcessCardPaymentCommandError.UnexpectedException => StatusCode((int)HttpStatusCode.InternalServerError,
                    "An unexpected exception occurred"),
                _ => StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected exception occurred")
            };
        }

        if (result.Value.Status is PaymentStatus.Declined)
        {
            return StatusCode((int)HttpStatusCode.PaymentRequired, "The payment was declined");
        }

        return Ok(result.Value);
    }
}