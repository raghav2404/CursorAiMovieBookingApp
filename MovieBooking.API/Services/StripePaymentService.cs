using Microsoft.EntityFrameworkCore;
using MovieBooking.API.Data;
using MovieBooking.API.DTOs;
using MovieBooking.API.Models;
using Stripe;
using Stripe.Checkout;

namespace MovieBooking.API.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly MovieBookingContext _context;
        private readonly string? _webhookSecret;

        public StripePaymentService(
            MovieBookingContext context,
            IConfiguration configuration)
        {
            _context = context;
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"] ?? 
                throw new InvalidOperationException("Stripe:SecretKey is not configured");
            _webhookSecret = configuration["Stripe:WebhookSecret"] ?? 
                throw new InvalidOperationException("Stripe:WebhookSecret is not configured");
        }

        public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found");

            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Booking is not in pending state");

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(booking.TotalAmount * 100), // Convert to cents
                Currency = "usd",
                Metadata = new Dictionary<string, string>
                {
                    { "bookingId", bookingId.ToString() },
                    { "movieTitle", booking.ShowTime.Movie.Title },
                    { "showTime", booking.ShowTime.StartTime.ToString("g") }
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            booking.PaymentIntentId = intent.Id;
            await _context.SaveChangesAsync();

            return new PaymentIntentResponseDto
            {
                ClientSecret = intent.ClientSecret,
                PaymentIntentId = intent.Id,
                Amount = booking.TotalAmount,
                Currency = "usd"
            };
        }

        public async Task<bool> ConfirmPaymentAsync(int bookingId, string paymentIntentId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");

            if (booking.PaymentIntentId != paymentIntentId)
                throw new InvalidOperationException("Payment intent ID mismatch");

            var service = new PaymentIntentService();
            var intent = await service.GetAsync(paymentIntentId);

            if (intent.Status == "succeeded")
            {
                booking.Status = BookingStatus.Confirmed;
                booking.TransactionId = intent.Id;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> HandleWebhookAsync(string json, string stripeSignature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _webhookSecret
                );

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    if (paymentIntent.Metadata.TryGetValue("bookingId", out var bookingIdStr) &&
                        int.TryParse(bookingIdStr, out var bookingId))
                    {
                        var booking = await _context.Bookings.FindAsync(bookingId);
                        if (booking != null)
                        {
                            booking.Status = BookingStatus.Confirmed;
                            booking.TransactionId = paymentIntent.Id;
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                else if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    if (paymentIntent.Metadata.TryGetValue("bookingId", out var bookingIdStr) &&
                        int.TryParse(bookingIdStr, out var bookingId))
                    {
                        var booking = await _context.Bookings.FindAsync(bookingId);
                        if (booking != null)
                        {
                            booking.Status = BookingStatus.Failed;
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                return true;
            }
            catch (StripeException)
            {
                return false;
            }
        }
    }
} 