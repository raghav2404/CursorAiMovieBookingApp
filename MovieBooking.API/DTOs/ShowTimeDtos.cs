using System;
using System.Collections.Generic;

namespace MovieBooking.API.DTOs
{
    public class ShowTimeCreateDto
    {
        public int MovieId { get; set; }
        public int TheaterId { get; set; }
        public DateTime StartTime { get; set; }
        public decimal BasePrice { get; set; }
    }

    public class ShowTimeUpdateDto : ShowTimeCreateDto
    {
        public bool IsActive { get; set; }
    }

    public class ShowTimeResponseDto : ShowTimeUpdateDto
    {
        public int Id { get; set; }
        public DateTime EndTime { get; set; }
        public MovieListItemDto Movie { get; set; } = null!;
        public TheaterBasicDto Theater { get; set; } = null!;
        public List<SeatResponseDto> AvailableSeats { get; set; } = new();
    }

    public class TheaterBasicDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class ShowTimeSearchDto
    {
        public DateTime? Date { get; set; }
        public int? MovieId { get; set; }
        public int? TheaterId { get; set; }
        public string? Location { get; set; }
    }
} 