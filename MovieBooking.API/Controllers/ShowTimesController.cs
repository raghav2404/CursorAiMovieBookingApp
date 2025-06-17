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
    public class ShowTimesController : ControllerBase
    {
        private readonly MovieBookingContext _context;
        private readonly IShowTimeService _showTimeService;

        public ShowTimesController(MovieBookingContext context, IShowTimeService showTimeService)
        {
            _context = context;
            _showTimeService = showTimeService;
        }

        // GET: api/ShowTimes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowTimeResponseDto>>> GetShowTimes([FromQuery] ShowTimeSearchDto searchDto)
        {
            var showTimes = await _showTimeService.GetShowTimesAsync(searchDto);
            return Ok(showTimes);
        }

        // GET: api/ShowTimes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShowTimeResponseDto>> GetShowTime(int id)
        {
            var showTime = await _showTimeService.GetShowTimeByIdAsync(id);
            if (showTime == null)
                return NotFound();

            return Ok(showTime);
        }

        // GET: api/ShowTimes/Movie/5
        [HttpGet("movie/{movieId}")]
        public async Task<ActionResult<IEnumerable<ShowTimeResponseDto>>> GetShowTimesByMovie(int movieId, [FromQuery] DateTime? date)
        {
            var showTimes = await _showTimeService.GetShowTimesByMovieAsync(movieId, date);
            return Ok(showTimes);
        }

        // POST: api/ShowTimes
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ShowTimeResponseDto>> CreateShowTime(ShowTimeCreateDto showTimeDto)
        {
            try
            {
                var showTime = await _showTimeService.CreateShowTimeAsync(showTimeDto);
                return CreatedAtAction(nameof(GetShowTime), new { id = showTime.Id }, showTime);
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

        // PUT: api/ShowTimes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ShowTimeResponseDto>> UpdateShowTime(int id, ShowTimeUpdateDto showTimeDto)
        {
            try
            {
                var showTime = await _showTimeService.UpdateShowTimeAsync(id, showTimeDto);
                return Ok(showTime);
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

        // DELETE: api/ShowTimes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShowTime(int id)
        {
            var result = await _showTimeService.DeleteShowTimeAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("validate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> ValidateShowTimeAvailability(
            [FromQuery] int theaterId,
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime)
        {
            var isAvailable = await _showTimeService.ValidateShowTimeAvailabilityAsync(theaterId, startTime, endTime);
            return Ok(isAvailable);
        }

        private bool ShowTimeExists(int id)
        {
            return _context.ShowTimes.Any(e => e.Id == id);
        }
    }
} 