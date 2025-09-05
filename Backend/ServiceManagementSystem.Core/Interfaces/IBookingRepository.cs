using ServiceManagementSystem.Core.Entities;

namespace ServiceManagementSystem.Core.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId);
        Task<IEnumerable<Booking>> GetBookingsByProviderAsync(int providerId);
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(string status);
        Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> IsTimeSlotAvailableAsync(int providerId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);
    }
}
