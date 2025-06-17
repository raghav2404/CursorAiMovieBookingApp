using MovieBooking.API.DTOs;

namespace MovieBooking.API.Services
{
    public interface IPaymentService
    {
        Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int bookingId);
        Task<bool> ConfirmPaymentAsync(int bookingId, string paymentIntentId);
        Task<bool> HandleWebhookAsync(string json, string stripeSignature);
    }
} 