using System.Collections.Generic;

namespace MovieBooking.API.DTOs
{
    public class TheaterCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalScreens { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class TheaterUpdateDto : TheaterCreateDto
    {
        public bool IsActive { get; set; }
    }

    public class TheaterResponseDto : TheaterUpdateDto
    {
        public int Id { get; set; }
        public List<ScreenResponseDto> Screens { get; set; } = new();
        public List<ShowTimeResponseDto> ShowTimes { get; set; } = new();
    }

    public class ScreenCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }

    public class ScreenResponseDto : ScreenCreateDto
    {
        public int Id { get; set; }
        public int TheaterId { get; set; }
        public bool IsActive { get; set; }
        public List<SeatResponseDto> Seats { get; set; } = new();
    }

    public class SeatCreateDto
    {
        public string SeatNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal PriceMultiplier { get; set; }
    }

    public class SeatResponseDto : SeatCreateDto
    {
        public int Id { get; set; }
        public int ScreenId { get; set; }
        public bool IsActive { get; set; }
    }
} 