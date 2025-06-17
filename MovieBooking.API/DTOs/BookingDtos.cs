using System;
using System.Collections.Generic;
using MovieBooking.API.Models;

namespace MovieBooking.API.DTOs
{
    public class BookingCreateDto
    {
        public int ShowTimeId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<int> SeatIds { get; set; } = new();
    }

    public class BookingResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime BookingTime { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public ShowTimeBookingDto ShowTime { get; set; } = null!;
        public List<BookedSeatResponseDto> BookedSeats { get; set; } = new();
    }

    public class ShowTimeBookingDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public MovieListItemDto Movie { get; set; } = null!;
        public TheaterBasicDto Theater { get; set; } = null!;
    }

    public class BookedSeatResponseDto
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class BookingStatusUpdateDto
    {
        public BookingStatus Status { get; set; }
    }
} 