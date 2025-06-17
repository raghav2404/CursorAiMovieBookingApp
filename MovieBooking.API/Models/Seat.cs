namespace MovieBooking.API.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int ScreenId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public SeatType Type { get; set; }
        public decimal PriceMultiplier { get; set; }
        public bool IsActive { get; set; }

        public virtual Screen Screen { get; set; } = null!;
    }

    public enum SeatType
    {
        Standard,
        Premium,
        Recliner,
        Couple
    }
}
