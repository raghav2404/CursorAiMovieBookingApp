using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.API.DTOs;
using MovieBooking.API.Services;
using System.IO;

namespace MovieBooking.API.Controllers
{
    /// <summary>
    /// Controller for handling payment-related operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Initializes a new instance of the PaymentsController
        /// </summary>
        /// <param name="paymentService">The payment service instance</param>
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Creates a new payment intent for a booking
        /// </summary>
        /// <param name="bookingId">The ID of the booking to create a payment for</param>
        /// <returns>Payment intent details including client secret</returns>
        /// <response code="200">Returns the payment intent details</response>
        /// <response code="400">If the booking is invalid or not in pending state</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the booking is not found</response>
        [HttpPost("create-payment-intent")]
        [Authorize]
        [ProducesResponseType(typeof(PaymentIntentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentIntentResponseDto>> CreatePaymentIntent([FromBody] int bookingId)
        {
            try
            {
                var paymentIntent = await _paymentService.CreatePaymentIntentAsync(bookingId);
                return Ok(paymentIntent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Confirms a payment for a booking
        /// </summary>
        /// <param name="confirmation">The payment confirmation details</param>
        /// <returns>No content if successful</returns>
        /// <response code="200">If the payment is confirmed successfully</response>
        /// <response code="400">If the payment confirmation fails</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the booking is not found</response>
        [HttpPost("confirm-payment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentConfirmationDto confirmation)
        {
            try
            {
                var result = await _paymentService.ConfirmPaymentAsync(confirmation.BookingId, confirmation.PaymentIntentId);
                if (result)
                    return Ok();
                return BadRequest(new { message = "Payment confirmation failed" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Handles Stripe webhook events
        /// </summary>
        /// <returns>OK if the webhook is processed successfully</returns>
        /// <response code="200">If the webhook is processed successfully</response>
        /// <response code="400">If the webhook signature is invalid or processing fails</response>
        [HttpPost("webhook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            if (!Request.Headers.TryGetValue("Stripe-Signature", out var stripeSignature))
            {
                return BadRequest(new { message = "Missing Stripe signature" });
            }

            var success = await _paymentService.HandleWebhookAsync(json, stripeSignature.ToString());
            if (success)
                return Ok();

            return BadRequest();
        }
    }
} 