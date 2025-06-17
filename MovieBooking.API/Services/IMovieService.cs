using MovieBooking.API.DTOs;
using MovieBooking.API.Models;

namespace MovieBooking.API.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieListItemDto>> GetAllMoviesAsync();
        Task<MovieResponseDto?> GetMovieByIdAsync(int id);
        Task<MovieResponseDto> CreateMovieAsync(MovieCreateDto movieDto);
        Task<MovieResponseDto> UpdateMovieAsync(int id, MovieUpdateDto movieDto);
        Task<bool> DeleteMovieAsync(int id);
    }
} 