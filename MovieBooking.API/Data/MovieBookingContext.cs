using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieBooking.API.Models;

namespace MovieBooking.API.Data
{
    public class MovieBookingContext : IdentityDbContext<ApplicationUser>
    {
        public MovieBookingContext(DbContextOptions<MovieBookingContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<ShowTime> ShowTimes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<SeatLock> SeatLocks { get; set; }
        public DbSet<BookedSeat> BookedSeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision
            modelBuilder.Entity<BookedSeat>()
                .Property(bs => bs.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Seat>()
                .Property(s => s.PriceMultiplier)
                .HasPrecision(4, 2);

            modelBuilder.Entity<ShowTime>()
                .Property(st => st.BasePrice)
                .HasPrecision(10, 2);

            // Configure relationships
            modelBuilder.Entity<Theater>()
                .HasMany(t => t.Screens)
                .WithOne(s => s.Theater)
                .HasForeignKey(s => s.TheaterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Screen>()
                .HasMany(s => s.Seats)
                .WithOne(s => s.Screen)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShowTime>()
                .HasOne(st => st.Movie)
                .WithMany(m => m.ShowTimes)
                .HasForeignKey(st => st.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShowTime>()
                .HasOne(st => st.Theater)
                .WithMany(t => t.ShowTimes)
                .HasForeignKey(st => st.TheaterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ShowTime)
                .WithMany(st => st.Bookings)
                .HasForeignKey(b => b.ShowTimeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<BookedSeat>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookedSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookedSeat>()
                .HasOne(bs => bs.Seat)
                .WithMany()
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Configure soft delete query filters
            modelBuilder.Entity<Movie>().HasQueryFilter(m => m.IsActive);
            modelBuilder.Entity<Theater>().HasQueryFilter(t => t.IsActive);
            modelBuilder.Entity<Screen>().HasQueryFilter(s => s.IsActive);
            modelBuilder.Entity<Seat>().HasQueryFilter(s => s.IsActive);
            modelBuilder.Entity<ShowTime>().HasQueryFilter(st => st.IsActive);

            // Configure query filter for Booking and BookedSeat
            modelBuilder.Entity<Booking>().HasQueryFilter(b => b.ShowTime == null || b.ShowTime.IsActive);
            modelBuilder.Entity<BookedSeat>().HasQueryFilter(bs => bs.Seat == null || bs.Seat.IsActive);
        }
    }
} 