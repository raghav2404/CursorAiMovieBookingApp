using MovieBooking.API.DTOs;

namespace MovieBooking.API.Services
{
    public interface IShowTimeService
    {
        Task<IEnumerable<ShowTimeResponseDto>> GetShowTimesAsync(ShowTimeSearchDto searchDto);
        Task<ShowTimeResponseDto?> GetShowTimeByIdAsync(int id);
        Task<ShowTimeResponseDto> CreateShowTimeAsync(ShowTimeCreateDto showTimeDto);
        Task<ShowTimeResponseDto> UpdateShowTimeAsync(int id, ShowTimeUpdateDto showTimeDto);
        Task<bool> DeleteShowTimeAsync(int id);
        Task<IEnumerable<ShowTimeResponseDto>> GetShowTimesByMovieAsync(int movieId, DateTime? date);
        Task<bool> ValidateShowTimeAvailabilityAsync(int theaterId, DateTime startTime, DateTime endTime, int? excludeShowTimeId = null);
    }
} 
 