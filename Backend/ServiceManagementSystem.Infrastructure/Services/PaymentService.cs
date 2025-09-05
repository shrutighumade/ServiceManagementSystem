using AutoMapper;
using ServiceManagementSystem.Core.DTOs;
using ServiceManagementSystem.Core.Entities;
using ServiceManagementSystem.Core.Interfaces;

namespace ServiceManagementSystem.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            return payment != null ? _mapper.Map<PaymentDto>(payment) : null;
        }

        public async Task<PaymentDto?> GetPaymentByBookingIdAsync(int bookingId)
        {
            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);
            return payment != null ? _mapper.Map<PaymentDto>(payment) : null;
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status)
        {
            var payments = await _unitOfWork.Payments.FindAsync(p => p.Status == status);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto> ProcessPaymentAsync(int bookingId, string paymentMethod, decimal amount)
        {
            // Check if payment already exists for this booking
            var existingPayment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);
            if (existingPayment != null)
            {
                throw new InvalidOperationException("Payment already exists for this booking");
            }

            // Generate a dummy transaction ID
            var transactionId = $"TXN_{DateTime.UtcNow:yyyyMMddHHmmss}_{bookingId}";

            // Simulate payment processing with random success/failure
            var random = new Random();
            var isSuccess = random.Next(1, 11) <= 8; // 80% success rate

            var payment = new Payment
            {
                BookingId = bookingId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId,
                Status = isSuccess ? "Success" : "Failed",
                FailureReason = isSuccess ? null : "Payment gateway declined the transaction",
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow
            };

            await _unitOfWork.Payments.AddAsync(payment);

            // Update booking status based on payment result
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking != null)
            {
                booking.Status = isSuccess ? "Confirmed" : "PaymentFailed";
                booking.UpdatedAt = DateTime.UtcNow;
                if (isSuccess)
                {
                    booking.ConfirmedAt = DateTime.UtcNow;
                }
                await _unitOfWork.Bookings.UpdateAsync(booking);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> RefundPaymentAsync(int paymentId, string reason)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new KeyNotFoundException("Payment not found");
            }

            if (payment.Status != "Success")
            {
                throw new InvalidOperationException("Only successful payments can be refunded");
            }

            payment.Status = "Refunded";
            payment.FailureReason = reason;
            payment.ProcessedAt = DateTime.UtcNow;

            await _unitOfWork.Payments.UpdateAsync(payment);

            // Update booking status
            var booking = await _unitOfWork.Bookings.GetByIdAsync(payment.BookingId);
            if (booking != null)
            {
                booking.Status = "Refunded";
                booking.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Bookings.UpdateAsync(booking);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<bool> ValidatePaymentAsync(string transactionId)
        {
            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
            return payment != null && payment.Status == "Success";
        }
    }
}
