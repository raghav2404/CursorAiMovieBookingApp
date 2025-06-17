using System;
using System.Collections.Generic;

namespace MovieBooking.API.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }
}
