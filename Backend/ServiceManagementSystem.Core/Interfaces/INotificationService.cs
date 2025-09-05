namespace ServiceManagementSystem.Core.Interfaces
{
    public interface INotificationService
    {
        Task SendBookingNotificationAsync(int bookingId, string message);
        Task SendPaymentNotificationAsync(int paymentId, string message);
        Task SendStatusUpdateNotificationAsync(int bookingId, string newStatus);
        Task NotifyProviderNewBookingAsync(int providerId, int bookingId);
        Task NotifyUserBookingUpdateAsync(int userId, int bookingId, string status);
    }
}
