using Microsoft.EntityFrameworkCore;
using MovieBooking.API.Data;
using MovieBooking.API.DTOs;
using MovieBooking.API.Models;

namespace MovieBooking.API.Services
{
    public class TheaterService : ITheaterService
    {
        private readonly MovieBookingContext _context;

        public TheaterService(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TheaterResponseDto>> GetAllTheatersAsync()
        {
            var theaters = await _context.Theaters
                .Include(t => t.Screens.Where(s => s.IsActive))
                .Include(t => t.ShowTimes.Where(st => st.IsActive))
                .Where(t => t.IsActive)
                .Select(t => MapToTheaterResponseDto(t))
                .ToListAsync();

            return theaters;
        }

        public async Task<TheaterResponseDto?> GetTheaterByIdAsync(int id)
        {
            var theater = await _context.Theaters
                .Include(t => t.Screens.Where(s => s.IsActive))
                    .ThenInclude(s => s.Seats.Where(seat => seat.IsActive))
                .Include(t => t.ShowTimes.Where(st => st.IsActive))
                .FirstOrDefaultAsync(t => t.Id == id);

            if (theater == null)
                return null;

            return MapToTheaterResponseDto(theater);
        }

        public async Task<TheaterResponseDto> CreateTheaterAsync(TheaterCreateDto theaterDto)
        {
            var theater = new Theater
            {
                Name = theaterDto.Name,
                Location = theaterDto.Location,
                Address = theaterDto.Address,
                TotalScreens = theaterDto.TotalScreens,
                ContactNumber = theaterDto.ContactNumber,
                Email = theaterDto.Email,
                IsActive = true
            };

            _context.Theaters.Add(theater);
            await _context.SaveChangesAsync();

            return await GetTheaterByIdAsync(theater.Id) ?? throw new Exception("Failed to create theater");
        }

        public async Task<TheaterResponseDto> UpdateTheaterAsync(int id, TheaterUpdateDto theaterDto)
        {
            var theater = await _context.Theaters.FindAsync(id);
            if (theater == null)
                throw new KeyNotFoundException($"Theater with ID {id} not found");

            theater.Name = theaterDto.Name;
            theater.Location = theaterDto.Location;
            theater.Address = theaterDto.Address;
            theater.TotalScreens = theaterDto.TotalScreens;
            theater.ContactNumber = theaterDto.ContactNumber;
            theater.Email = theaterDto.Email;
            theater.IsActive = theaterDto.IsActive;

            await _context.SaveChangesAsync();

            return await GetTheaterByIdAsync(id) ?? throw new Exception("Failed to update theater");
        }

        public async Task<bool> DeleteTheaterAsync(int id)
        {
            var theater = await _context.Theaters.FindAsync(id);
            if (theater == null)
                return false;

            theater.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ScreenResponseDto> AddScreenAsync(int theaterId, ScreenCreateDto screenDto)
        {
            var theater = await _context.Theaters.FindAsync(theaterId);
            if (theater == null)
                throw new KeyNotFoundException($"Theater with ID {theaterId} not found");

            var screen = new Screen
            {
                TheaterId = theaterId,
                Name = screenDto.Name,
                Capacity = screenDto.Capacity,
                IsActive = true
            };

            _context.Screens.Add(screen);
            await _context.SaveChangesAsync();

            return MapToScreenResponseDto(screen);
        }

        public async Task<IEnumerable<ScreenResponseDto>> GetScreensByTheaterIdAsync(int theaterId)
        {
            var screens = await _context.Screens
                .Include(s => s.Seats.Where(seat => seat.IsActive))
                .Where(s => s.TheaterId == theaterId && s.IsActive)
                .Select(s => MapToScreenResponseDto(s))
                .ToListAsync();

            return screens;
        }

        private static TheaterResponseDto MapToTheaterResponseDto(Theater theater)
        {
            return new TheaterResponseDto
            {
                Id = theater.Id,
                Name = theater.Name,
                Location = theater.Location,
                Address = theater.Address,
                TotalScreens = theater.TotalScreens,
                ContactNumber = theater.ContactNumber,
                Email = theater.Email,
                IsActive = theater.IsActive,
                Screens = theater.Screens.Select(s => MapToScreenResponseDto(s)).ToList(),
                ShowTimes = theater.ShowTimes.Select(st => new ShowTimeResponseDto
                {
                    Id = st.Id,
                    MovieId = st.MovieId,
                    TheaterId = st.TheaterId,
                    StartTime = st.StartTime,
                    EndTime = st.EndTime,
                    BasePrice = st.BasePrice,
                    IsActive = st.IsActive
                }).ToList()
            };
        }

        private static ScreenResponseDto MapToScreenResponseDto(Screen screen)
        {
            return new ScreenResponseDto
            {
                Id = screen.Id,
                TheaterId = screen.TheaterId,
                Name = screen.Name,
                Capacity = screen.Capacity,
                IsActive = screen.IsActive,
                Seats = screen.Seats?.Select(s => new SeatResponseDto
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