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
    public class TheatersController : ControllerBase
    {
        private readonly MovieBookingContext _context;
        private readonly ITheaterService _theaterService;

        public TheatersController(MovieBookingContext context, ITheaterService theaterService)
        {
            _context = context;
            _theaterService = theaterService;
        }

        // GET: api/Theaters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheaterResponseDto>>> GetTheaters()
        {
            var theaters = await _theaterService.GetAllTheatersAsync();
            return Ok(theaters);
        }

        // GET: api/Theaters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TheaterResponseDto>> GetTheater(int id)
        {
            var theater = await _theaterService.GetTheaterByIdAsync(id);
            if (theater == null)
                return NotFound();

            return Ok(theater);
        }

        // POST: api/Theaters
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TheaterResponseDto>> CreateTheater(TheaterCreateDto theaterDto)
        {
            try
            {
                var theater = await _theaterService.CreateTheaterAsync(theaterDto);
                return CreatedAtAction(nameof(GetTheater), new { id = theater.Id }, theater);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Theaters/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TheaterResponseDto>> UpdateTheater(int id, TheaterUpdateDto theaterDto)
        {
            try
            {
                var theater = await _theaterService.UpdateTheaterAsync(id, theaterDto);
                return Ok(theater);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Theaters/5/Screens
        [HttpPost("{theaterId}/screens")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ScreenResponseDto>> AddScreen(int theaterId, ScreenCreateDto screenDto)
        {
            try
            {
                var screen = await _theaterService.AddScreenAsync(theaterId, screenDto);
                return CreatedAtAction(nameof(GetTheater), new { id = theaterId }, screen);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Theaters/5/Screens
        [HttpGet("{theaterId}/screens")]
        public async Task<ActionResult<IEnumerable<ScreenResponseDto>>> GetScreens(int theaterId)
        {
            try
            {
                var screens = await _theaterService.GetScreensByTheaterIdAsync(theaterId);
                return Ok(screens);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Theaters/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTheater(int id)
        {
            var result = await _theaterService.DeleteTheaterAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        private bool TheaterExists(int id)
        {
            return _context.Theaters.Any(e => e.Id == id);
        }
    }
} 