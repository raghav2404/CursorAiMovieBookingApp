using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieBooking.API.Services
{
    public interface ISeatLockService
    {
        Task<bool> LockSeatsAsync(int showTimeId, List<int> seatIds, string userId);
        Task<bool> UnlockSeatsAsync(int showTimeId, List<int> seatIds, string userId);
        Task<bool> ValidateAndExtendLocksAsync(int showTimeId, List<int> seatIds, string userId);
        Task<bool> AreSeatsAvailableAsync(int showTimeId, List<int> seatIds);
        Task CleanupExpiredLocksAsync();
    }
} 