using System;
using System.Collections.Generic;

namespace MovieBooking.API.DTOs
{
    public class MovieCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
    }

    public class MovieUpdateDto : MovieCreateDto
    {
        public bool IsActive { get; set; }
    }

    public class MovieResponseDto : MovieUpdateDto
    {
        public int Id { get; set; }
        public List<ShowTimeResponseDto> ShowTimes { get; set; } = new();
    }

    public class MovieListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
    }
} 