using System;
using System.Collections.Generic;

namespace MovieBooking.API.Models
{
    public class ShowTime
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int TheaterId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }

        public virtual Movie Movie { get; set; } = null!;
        public virtual Theater Theater { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Seat> AvailableSeats { get; set; } = new List<Seat>();
    }
}
