using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieBooking.API.Data;
using MovieBooking.API.Models;

namespace MovieBooking.API.Services
{
    public class SeatLockService : ISeatLockService
    {
        private readonly MovieBookingContext _context;
        private readonly TimeSpan _lockDuration = TimeSpan.FromMinutes(10);

        public SeatLockService(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<bool> LockSeatsAsync(int showTimeId, List<int> seatIds, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if seats are available
                if (!await AreSeatsAvailableAsync(showTimeId, seatIds))
                {
                    return false;
                }

                var showTime = await _context.ShowTimes.FindAsync(showTimeId);
                var seats = await _context.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync();

                if (showTime == null || seats.Count != seatIds.Count)
                {
                    return false;
                }

                var now = DateTime.UtcNow;
                var locks = seats.Select(seat => new SeatLock
                {
                    ShowTimeId = showTimeId,
                    ShowTime = showTime,
                    SeatId = seat.Id,
                    Seat = seat,
                    UserId = userId,
                    LockTime = now,
                    ExpiryTime = now.Add(_lockDuration)
                }).ToList();

                await _context.SeatLocks.AddRangeAsync(locks);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UnlockSeatsAsync(int showTimeId, List<int> seatIds, string userId)
        {
            var locks = await _context.SeatLocks
                .Where(sl => sl.ShowTimeId == showTimeId && 
                            seatIds.Contains(sl.SeatId) && 
                            sl.UserId == userId)
                .ToListAsync();

            if (!locks.Any())
                return false;

            _context.SeatLocks.RemoveRange(locks);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateAndExtendLocksAsync(int showTimeId, List<int> seatIds, string userId)
        {
            var now = DateTime.UtcNow;
            var locks = await _context.SeatLocks
                .Where(sl => sl.ShowTimeId == showTimeId && 
                            seatIds.Contains(sl.SeatId) && 
                            sl.UserId == userId &&
                            sl.ExpiryTime > now)
                .ToListAsync();

            if (locks.Count != seatIds.Count)
                return false;

            foreach (var seatLock in locks)
            {
                seatLock.ExpiryTime = now.Add(_lockDuration);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AreSeatsAvailableAsync(int showTimeId, List<int> seatIds)
        {
            var now = DateTime.UtcNow;

            // Check for existing locks
            var lockedSeats = await _context.SeatLocks
                .Where(sl => sl.ShowTimeId == showTimeId && 
                            seatIds.Contains(sl.SeatId) && 
                            sl.ExpiryTime > now)
                .AnyAsync();

            if (lockedSeats)
                return false;

            // Check if seats are already booked
            var bookedSeats = await _context.BookedSeats
                .Where(bs => bs.Booking.ShowTimeId == showTimeId && 
                            seatIds.Contains(bs.SeatId) && 
                            bs.Booking.Status != BookingStatus.Cancelled)
                .AnyAsync();

            return !bookedSeats;
        }

        public async Task CleanupExpiredLocksAsync()
        {
            var expiredLocks = await _context.SeatLocks
                .Where(sl => sl.ExpiryTime <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredLocks.Any())
            {
                _context.SeatLocks.RemoveRange(expiredLocks);
                await _context.SaveChangesAsync();
            }
        }
    }
} 