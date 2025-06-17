using System.Collections.Generic;

namespace MovieBooking.API.Models
{
    public class Theater
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalScreens { get; set; }
        public bool IsActive { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
        public virtual ICollection<Screen> Screens { get; set; } = new List<Screen>();
    }

    public class Screen
    {
        public int Id { get; set; }
        public int TheaterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsActive { get; set; }

        public virtual Theater Theater { get; set; } = null!;
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
