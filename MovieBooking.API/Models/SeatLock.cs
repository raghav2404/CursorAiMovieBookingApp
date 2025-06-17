using System;

namespace MovieBooking.API.Models
{
    public class SeatLock
    {
        public int Id { get; set; }
        public int SeatId { get; set; }
        public int ShowTimeId { get; set; }
        public required string UserId { get; set; }
        public DateTime LockTime { get; set; }
        public DateTime ExpiryTime { get; set; }

        // Navigation properties
        public required Seat Seat { get; set; }
        public required ShowTime ShowTime { get; set; }
    }
} 