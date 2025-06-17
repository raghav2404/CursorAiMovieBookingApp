using System.ComponentModel.DataAnnotations;

namespace MovieBooking.API.DTOs
{
    public class PaymentConfirmationDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public required string PaymentIntentId { get; set; }
    }
} 