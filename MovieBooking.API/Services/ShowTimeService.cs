using Microsoft.EntityFrameworkCore;
using MovieBooking.API.Data;
using MovieBooking.API.DTOs;
using MovieBooking.API.Models;

namespace MovieBooking.API.Services
{
    public class ShowTimeService : IShowTimeService
    {
        private readonly MovieBookingContext _context;

        public ShowTimeService(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShowTimeResponseDto>> GetShowTimesAsync(ShowTimeSearchDto searchDto)
        {
            var query = _context.ShowTimes
                .Include(st => st.Movie)
                .Include(st => st.Theater)
                .Include(st => st.AvailableSeats)
                .Where(st => st.IsActive);

            if (searchDto.Date.HasValue)
            {
                var startDate = searchDto.Date.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(st => st.StartTime >= startDate && st.StartTime < endDate);
            }

            if (searchDto.MovieId.HasValue)
            {
                query = query.Where(st => st.MovieId == searchDto.MovieId);
            }

            if (searchDto.TheaterId.HasValue)
            {
                query = query.Where(st => st.TheaterId == searchDto.TheaterId);
            }

            if (!string.IsNullOrEmpty(searchDto.Location))
            {
                query = query.Where(st => st.Theater.Location.Contains(searchDto.Location));
            }

            var showTimes = await query.Select(st => MapToShowTimeResponseDto(st)).ToListAsync();
            return showTimes;
        }

        public async Task<ShowTimeResponseDto?> GetShowTimeByIdAsync(int id)
        {
            var showTime = await _context.ShowTimes
                .Include(st => st.Movie)
                .Include(st => st.Theater)
                .Include(st => st.AvailableSeats)
                .FirstOrDefaultAsync(st => st.Id == id);

            if (showTime == null)
                return null;

            return MapToShowTimeResponseDto(showTime);
        }

        public async Task<ShowTimeResponseDto> CreateShowTimeAsync(ShowTimeCreateDto showTimeDto)
        {
            var movie = await _context.Movies.FindAsync(showTimeDto.MovieId);
            if (movie == null)
                throw new KeyNotFoundException($"Movie with ID {showTimeDto.MovieId} not found");

            var endTime = showTimeDto.StartTime.AddMinutes(movie.DurationMinutes);

            var isAvailable = await ValidateShowTimeAvailabilityAsync(
                showTimeDto.TheaterId,
                showTimeDto.StartTime,
                endTime);

            if (!isAvailable)
                throw new InvalidOperationException("The selected time slot is not available");

            var showTime = new ShowTime
            {
                MovieId = showTimeDto.MovieId,
                TheaterId = showTimeDto.TheaterId,
                StartTime = showTimeDto.StartTime,
                EndTime = endTime,
                BasePrice = showTimeDto.BasePrice,
                IsActive = true
            };

            _context.ShowTimes.Add(showTime);
            await _context.SaveChangesAsync();

            return await GetShowTimeByIdAsync(showTime.Id) ?? throw new Exception("Failed to create show time");
        }

        public async Task<ShowTimeResponseDto> UpdateShowTimeAsync(int id, ShowTimeUpdateDto showTimeDto)
        {
            var showTime = await _context.ShowTimes.FindAsync(id);
            if (showTime == null)
                throw new KeyNotFoundException($"ShowTime with ID {id} not found");

            var movie = await _context.Movies.FindAsync(showTimeDto.MovieId);
            if (movie == null)
                throw new KeyNotFoundException($"Movie with ID {showTimeDto.MovieId} not found");

            var endTime = showTimeDto.StartTime.AddMinutes(movie.DurationMinutes);

            if (showTime.TheaterId != showTimeDto.TheaterId || showTime.StartTime != showTimeDto.StartTime)
            {
                var isAvailable = await ValidateShowTimeAvailabilityAsync(
                    showTimeDto.TheaterId,
                    showTimeDto.StartTime,
                    endTime,
                    id);

                if (!isAvailable)
                    throw new InvalidOperationException("The selected time slot is not available");
            }

            showTime.MovieId = showTimeDto.MovieId;
            showTime.TheaterId = showTimeDto.TheaterId;
            showTime.StartTime = showTimeDto.StartTime;
            showTime.EndTime = endTime;
            showTime.BasePrice = showTimeDto.BasePrice;
            showTime.IsActive = showTimeDto.IsActive;

            await _context.SaveChangesAsync();

            return await GetShowTimeByIdAsync(id) ?? throw new Exception("Failed to update show time");
        }

        public async Task<bool> DeleteShowTimeAsync(int id)
        {
            var showTime = await _context.ShowTimes.FindAsync(id);
            if (showTime == null)
                return false;

            showTime.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ShowTimeResponseDto>> GetShowTimesByMovieAsync(int movieId, DateTime? date)
        {
            var query = _context.ShowTimes
                .Include(st => st.Theater)
                .Include(st => st.AvailableSeats)
                .Where(st => st.MovieId == movieId && st.IsActive);

            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(st => st.StartTime >= startDate && st.StartTime < endDate);
            }

            var showTimes = await query.Select(st => MapToShowTimeResponseDto(st)).ToListAsync();
            return showTimes;
        }

        public async Task<bool> ValidateShowTimeAvailabilityAsync(int theaterId, DateTime startTime, DateTime endTime, int? excludeShowTimeId = null)
        {
            var query = _context.ShowTimes
                .Where(st => st.TheaterId == theaterId &&
                            st.IsActive &&
                            ((startTime >= st.StartTime && startTime < st.EndTime) ||
                             (endTime > st.StartTime && endTime <= st.EndTime)));

            if (excludeShowTimeId.HasValue)
            {
                query = query.Where(st => st.Id != excludeShowTimeId.Value);
            }

            return !await query.AnyAsync();
        }

        private static ShowTimeResponseDto MapToShowTimeResponseDto(ShowTime showTime)
        {
            return new ShowTimeResponseDto
            {
                Id = showTime.Id,
                MovieId = showTime.MovieId,
                TheaterId = showTime.TheaterId,
                StartTime = showTime.StartTime,
                EndTime = showTime.EndTime,
                BasePrice = showTime.BasePrice,
                IsActive = showTime.IsActive,
                Movie = showTime.Movie != null ? new MovieListItemDto
                {
                    Id = showTime.Movie.Id,
                    Title = showTime.Movie.Title,
                    Language = showTime.Movie.Language,
                    Genre = showTime.Movie.Genre,
                    ReleaseDate = showTime.Movie.ReleaseDate,
                    PosterUrl = showTime.Movie.PosterUrl
                } : null!,
                Theater = showTime.Theater != null ? new TheaterBasicDto
                {
                    Id = showTime.Theater.Id,
                    Name = showTime.Theater.Name,
                    Location = showTime.Theater.Location
                } : null!,
                AvailableSeats = showTime.AvailableSeats?.Select(s => new SeatResponseDto
                {
                    Id = s.Id,
                    ScreenId = s.ScreenId,
                    SeatNumber = s.SeatNumber,
                    Type = s.Type.ToString(),
                    PriceMultiplier = s.PriceMultiplier,
                    IsActive = s.IsActive
                }).ToList() ?? new List<SeatResponseDto>()
            };
        }
    }
} 