using ServiceManagementSystem.Core.DTOs;

namespace ServiceManagementSystem.Core.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task<IEnumerable<BookingDto>> GetBookingsByUserAsync(int userId);
        Task<IEnumerable<BookingDto>> GetBookingsByProviderAsync(int providerId);
        Task<IEnumerable<BookingDto>> GetBookingsByStatusAsync(string status);
        Task<IEnumerable<BookingDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto, int userId);
        Task<BookingDto> UpdateBookingStatusAsync(int id, UpdateBookingStatusDto updateDto);
        Task<bool> CancelBookingAsync(int id, int userId);
        Task<bool> IsTimeSlotAvailableAsync(int providerId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);
        Task<PaymentDto> ProcessPaymentAsync(int bookingId, string paymentMethod);
    }
}
