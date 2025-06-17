using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieBooking.API.DTOs
{
    public class SeatLockRequestDto
    {
        [Required]
        public int ShowTimeId { get; set; }

        [Required]
        public List<int> SeatIds { get; set; } = new();
    }
} 