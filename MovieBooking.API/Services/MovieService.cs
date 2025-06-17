using Microsoft.EntityFrameworkCore;
using MovieBooking.API.Data;
using MovieBooking.API.DTOs;
using MovieBooking.API.Models;

namespace MovieBooking.API.Services
{
    public class MovieService : IMovieService
    {
        private readonly MovieBookingContext _context;

        public MovieService(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovieListItemDto>> GetAllMoviesAsync()
        {
            var movies = await _context.Movies
                .Where(m => m.IsActive)
                .Select(m => new MovieListItemDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Language = m.Language,
                    Genre = m.Genre,
                    ReleaseDate = m.ReleaseDate,
                    PosterUrl = m.PosterUrl
                })
                .ToListAsync();

            return movies;
        }

        public async Task<MovieResponseDto?> GetMovieByIdAsync(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.ShowTimes.Where(st => st.IsActive))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return null;

            return new MovieResponseDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Language = movie.Language,
                Genre = movie.Genre,
                DurationMinutes = movie.DurationMinutes,
                ReleaseDate = movie.ReleaseDate,
                PosterUrl = movie.PosterUrl,
                IsActive = movie.IsActive,
                ShowTimes = movie.ShowTimes.Select(st => new ShowTimeResponseDto
                {
                    Id = st.Id,
                    StartTime = st.StartTime,
                    EndTime = st.EndTime,
                    BasePrice = st.BasePrice,
                    IsActive = st.IsActive
                }).ToList()
            };
        }

        public async Task<MovieResponseDto> CreateMovieAsync(MovieCreateDto movieDto)
        {
            var movie = new Movie
            {
                Title = movieDto.Title,
                Description = movieDto.Description,
                Language = movieDto.Language,
                Genre = movieDto.Genre,
                DurationMinutes = movieDto.DurationMinutes,
                ReleaseDate = movieDto.ReleaseDate,
                PosterUrl = movieDto.PosterUrl,
                IsActive = true
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return await GetMovieByIdAsync(movie.Id) ?? throw new Exception("Failed to create movie");
        }

        public async Task<MovieResponseDto> UpdateMovieAsync(int id, MovieUpdateDto movieDto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                throw new KeyNotFoundException($"Movie with ID {id} not found");

            movie.Title = movieDto.Title;
            movie.Description = movieDto.Description;
            movie.Language = movieDto.Language;
            movie.Genre = movieDto.Genre;
            movie.DurationMinutes = movieDto.DurationMinutes;
            movie.ReleaseDate = movieDto.ReleaseDate;
            movie.PosterUrl = movieDto.PosterUrl;
            movie.IsActive = movieDto.IsActive;

            await _context.SaveChangesAsync();

            return await GetMovieByIdAsync(id) ?? throw new Exception("Failed to update movie");
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return false;

            movie.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 