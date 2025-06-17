using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBooking.API.Data;
using MovieBooking.API.DTOs;
using MovieBooking.API.Models;
using MovieBooking.API.Services;

namespace MovieBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly MovieBookingContext _context;
        private readonly IBookingService _bookingService;
        private readonly ISeatLockService _seatLockService;

        public BookingsController(MovieBookingContext context, IBookingService bookingService, ISeatLockService seatLockService)
        {
            _context = context;
            _bookingService = bookingService;
            _seatLockService = seatLockService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetUserBookings()
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId) || booking.UserId != userId)
                return Unauthorized();

            return Ok(booking);
        }

        [HttpPost("lock-seats")]
        public async Task<IActionResult> LockSeats([FromBody] SeatLockRequestDto request)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _seatLockService.LockSeatsAsync(request.ShowTimeId, request.SeatIds, userId);
            if (!result)
                return BadRequest(new { message = "Failed to lock seats. Seats might be unavailable." });

            return Ok();
        }

        [HttpPost("unlock-seats")]
        public async Task<IActionResult> UnlockSeats([FromBody] SeatLockRequestDto request)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _seatLockService.UnlockSeatsAsync(request.ShowTimeId, request.SeatIds, userId);
            if (!result)
                return BadRequest(new { message = "Failed to unlock seats" });

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking(BookingCreateDto bookingDto)
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                bookingDto.UserId = userId;
                var booking = await _bookingService.CreateBookingAsync(bookingDto);
                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] BookingStatus status)
        {
            var result = await _bookingService.UpdateBookingStatusAsync(id, status);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId) || booking.UserId != userId)
                return Unauthorized();

            var result = await _bookingService.CancelBookingAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("validate-seats")]
        public async Task<ActionResult<bool>> ValidateSeatAvailability([FromQuery] int showTimeId, [FromQuery] List<int> seatIds)
        {
            var isAvailable = await _bookingService.ValidateSeatAvailabilityAsync(showTimeId, seatIds);
            return Ok(isAvailable);
        }
    }
} 