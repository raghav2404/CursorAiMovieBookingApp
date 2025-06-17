using MovieBooking.API.DTOs;
using MovieBooking.API.Models;

namespace MovieBooking.API.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(string userId);
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<BookingResponseDto> CreateBookingAsync(BookingCreateDto bookingDto);
        Task<bool> UpdateBookingStatusAsync(int id, BookingStatus status);
        Task<bool> CancelBookingAsync(int id);
        Task<bool> ValidateSeatAvailabilityAsync(int showTimeId, IEnumerable<int> seatIds);
    }
} 