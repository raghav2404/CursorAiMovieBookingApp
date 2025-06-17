using System;
using System.Collections.Generic;

namespace MovieBooking.API.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int ShowTimeId { get; set; }
        public DateTime BookingTime { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? TransactionId { get; set; }

        public required ShowTime ShowTime { get; set; }
        public virtual ICollection<BookedSeat> BookedSeats { get; set; } = new List<BookedSeat>();
    }

    public class BookedSeat
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int SeatId { get; set; }
        public decimal Price { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Seat Seat { get; set; } = null!;
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Failed,
        Cancelled
    }
}
