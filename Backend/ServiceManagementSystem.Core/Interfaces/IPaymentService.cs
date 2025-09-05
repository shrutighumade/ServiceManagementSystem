using ServiceManagementSystem.Core.DTOs;

namespace ServiceManagementSystem.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto?> GetPaymentByIdAsync(int id);
        Task<PaymentDto?> GetPaymentByBookingIdAsync(int bookingId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status);
        Task<PaymentDto> ProcessPaymentAsync(int bookingId, string paymentMethod, decimal amount);
        Task<PaymentDto> RefundPaymentAsync(int paymentId, string reason);
        Task<bool> ValidatePaymentAsync(string transactionId);
    }
}
