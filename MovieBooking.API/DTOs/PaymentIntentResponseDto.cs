namespace MovieBooking.API.DTOs
{
    public class PaymentIntentResponseDto
    {
        public required string ClientSecret { get; set; }
        public required string PaymentIntentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
    }
} 