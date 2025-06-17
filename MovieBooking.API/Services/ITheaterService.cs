using MovieBooking.API.DTOs;

namespace MovieBooking.API.Services
{
    public interface ITheaterService
    {
        Task<IEnumerable<TheaterResponseDto>> GetAllTheatersAsync();
        Task<TheaterResponseDto?> GetTheaterByIdAsync(int id);
        Task<TheaterResponseDto> CreateTheaterAsync(TheaterCreateDto theaterDto);
        Task<TheaterResponseDto> UpdateTheaterAsync(int id, TheaterUpdateDto theaterDto);
        Task<bool> DeleteTheaterAsync(int id);
        Task<ScreenResponseDto> AddScreenAsync(int theaterId, ScreenCreateDto screenDto);
        Task<IEnumerable<ScreenResponseDto>> GetScreensByTheaterIdAsync(int theaterId);
    }
} 