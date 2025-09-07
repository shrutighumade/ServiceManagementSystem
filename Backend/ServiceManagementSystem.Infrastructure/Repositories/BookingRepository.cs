using Microsoft.EntityFrameworkCore;
using ServiceManagementSystem.Core.Entities;
using ServiceManagementSystem.Core.Interfaces;
using ServiceManagementSystem.Infrastructure.Data;

namespace ServiceManagementSystem.Infrastructure.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
        {
            return await _dbSet
                .Include(b => b.User)
                .Include(b => b.Provider)
                .ThenInclude(p => p.User)
                .Include(b => b.Service)
                .Include(b => b.Payment)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByProviderAsync(int providerId)
        {
            return await _dbSet
                .Include(b => b.User)
                .Include(b => b.Provider)
                .ThenInclude(p => p.User)
                .Include(b => b.Service)
                .Include(b => b.Payment)
                .Where(b => b.ProviderId == providerId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(string status)
        {
            return await _dbSet
                .Include(b => b.User)
                .Include(b => b.Provider)
                .ThenInclude(p => p.User)
                .Include(b => b.Service)
                .Include(b => b.Payment)
                .Where(b => b.Status == status)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(b => b.User)
                .Include(b => b.Provider)
                .ThenInclude(p => p.User)
                .Include(b => b.Service)
                .Include(b => b.Payment)
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int providerId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            Console.WriteLine($"Checking time slot availability: ProviderId={providerId}, Date={bookingDate.Date}, StartTime={startTime}, EndTime={endTime}");

            // First, get all potential conflicting bookings from the database
            var potentialConflicts = await _dbSet
                .Where(b => b.ProviderId == providerId &&
                           b.BookingDate.Date == bookingDate.Date &&
                           b.Status != "Cancelled" &&
                           b.Status != "Rejected")
                .ToListAsync();

            Console.WriteLine($"Found {potentialConflicts.Count} potential conflicting bookings");

            // Then check for time conflicts on the client side
            var conflictingBookings = potentialConflicts
                .Where(b => (b.StartTime <= startTime && b.EndTime > startTime) ||
                          (b.StartTime < endTime && b.EndTime >= endTime) ||
                          (b.StartTime >= startTime && b.EndTime <= endTime))
                .ToList();

            if (conflictingBookings.Any())
            {
                Console.WriteLine($"Found {conflictingBookings.Count} time slot conflicts:");
                foreach (var booking in conflictingBookings)
                {
                    Console.WriteLine($"  Conflicting booking ID={booking.Id}, StartTime={booking.StartTime}, EndTime={booking.EndTime}, Status={booking.Status}");
                }
            }
            else
            {
                Console.WriteLine("No time slot conflicts found");
            }

            return !conflictingBookings.Any();
        }
    }
}
