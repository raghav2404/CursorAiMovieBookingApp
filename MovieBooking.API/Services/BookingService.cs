using Microsoft.EntityFrameworkCore;
using MovieBooking.API.Data;
using MovieBooking.API.DTOs;
using MovieBooking.API.Models;

namespace MovieBooking.API.Services
{
    public class BookingService : IBookingService
    {
        private readonly MovieBookingContext _context;

        public BookingService(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Theater)
                .Include(b => b.BookedSeats)
                    .ThenInclude(bs => bs.Seat)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingTime)
                .Select(b => MapToBookingResponseDto(b))
                .ToListAsync();

            return bookings;
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Theater)
                .Include(b => b.BookedSeats)
                    .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return null;

            return MapToBookingResponseDto(booking);
        }

        public async Task<BookingResponseDto> CreateBookingAsync(BookingCreateDto bookingDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var showTime = await _context.ShowTimes
                    .Include(st => st.Movie)
                    .Include(st => st.Theater)
                    .FirstOrDefaultAsync(st => st.Id == bookingDto.ShowTimeId);

                if (showTime == null || !showTime.IsActive)
                    throw new KeyNotFoundException("Invalid show time");

                var isAvailable = await ValidateSeatAvailabilityAsync(bookingDto.ShowTimeId, bookingDto.SeatIds);
                if (!isAvailable)
                    throw new InvalidOperationException("One or more selected seats are not available");

                var seats = await _context.Seats
                    .Where(s => bookingDto.SeatIds.Contains(s.Id))
                    .ToListAsync();

                var booking = new Booking
                {
                    ShowTimeId = bookingDto.ShowTimeId,
                    ShowTime = showTime,
                    UserId = bookingDto.UserId,
                    BookingTime = DateTime.UtcNow,
                    Status = BookingStatus.Pending,
                    BookedSeats = seats.Select(seat => new BookedSeat
                    {
                        SeatId = seat.Id,
                        Price = showTime.BasePrice * seat.PriceMultiplier
                    }).ToList()
                };

                booking.TotalAmount = booking.BookedSeats.Sum(bs => bs.Price);

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetBookingByIdAsync(booking.Id) ?? throw new Exception("Failed to create booking");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateBookingStatusAsync(int id, BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return false;

            booking.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return false;

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateSeatAvailabilityAsync(int showTimeId, IEnumerable<int> seatIds)
        {
            var bookedSeats = await _context.BookedSeats
                .Include(bs => bs.Booking)
                .Where(bs => bs.Booking.ShowTimeId == showTimeId &&
                            bs.Booking.Status != BookingStatus.Cancelled &&
                            seatIds.Contains(bs.SeatId))
                .AnyAsync();

            return !bookedSeats;
        }

        private static BookingResponseDto MapToBookingResponseDto(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                BookingTime = booking.BookingTime,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,
                TransactionId = booking.TransactionId,
                ShowTime = new ShowTimeBookingDto
                {
                    Id = booking.ShowTime.Id,
                    StartTime = booking.ShowTime.StartTime,
                    EndTime = booking.ShowTime.EndTime,
                    Movie = new MovieListItemDto
                    {
                        Id = booking.ShowTime.Movie.Id,
                        Title = booking.ShowTime.Movie.Title,
                        Language = booking.ShowTime.Movie.Language,
                        Genre = booking.ShowTime.Movie.Genre,
                        ReleaseDate = booking.ShowTime.Movie.ReleaseDate,
                        PosterUrl = booking.ShowTime.Movie.PosterUrl
                    },
                    Theater = new TheaterBasicDto
                    {
                        Id = booking.ShowTime.Theater.Id,
                        Name = booking.ShowTime.Theater.Name,
                        Location = booking.ShowTime.Theater.Location
                    }
                },
                BookedSeats = booking.BookedSeats.Select(bs => new BookedSeatResponseDto
                {
                    SeatId = bs.SeatId,
                    SeatNumber = bs.Seat.SeatNumber,
                    Type = bs.Seat.Type.ToString(),
                    Price = bs.Price
                }).ToList()
            };
        }
    }
} 