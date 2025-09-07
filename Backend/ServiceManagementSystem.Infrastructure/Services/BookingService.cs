using AutoMapper;
using ServiceManagementSystem.Core.DTOs;
using ServiceManagementSystem.Core.Entities;
using ServiceManagementSystem.Core.Interfaces;

namespace ServiceManagementSystem.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentService = paymentService;
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            return booking != null ? _mapper.Map<BookingDto>(booking) : null;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByUserAsync(int userId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByUserAsync(userId);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByProviderAsync(int providerId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByProviderAsync(providerId);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByStatusAsync(string status)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByStatusAsync(status);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto, int userId)
        {
            // Get the service to get provider and pricing information
            var service = await _unitOfWork.Services.GetByIdAsync(createBookingDto.ServiceId);
            if (service == null)
            {
                throw new KeyNotFoundException("Service not found");
            }

            // Check if time slot is available
            var endTime = createBookingDto.StartTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));
            var isAvailable = await _unitOfWork.Bookings.IsTimeSlotAvailableAsync(
                service.ProviderId,
                createBookingDto.BookingDate,
                createBookingDto.StartTime,
                endTime);

            if (!isAvailable)
            {
                throw new InvalidOperationException("Time slot is not available");
            }

            var booking = _mapper.Map<Booking>(createBookingDto);
            booking.UserId = userId;
            booking.ProviderId = service.ProviderId;
            booking.EndTime = endTime;
            booking.TotalAmount = service.Price;
            booking.Status = "Pending";
            booking.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<BookingDto> UpdateBookingStatusAsync(int id, UpdateBookingStatusDto updateDto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found");
            }

            booking.Status = updateDto.Status;
            booking.UpdatedAt = DateTime.UtcNow;

            if (updateDto.Status == "Confirmed")
            {
                booking.ConfirmedAt = DateTime.UtcNow;
            }
            else if (updateDto.Status == "Completed")
            {
                booking.CompletedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<bool> CancelBookingAsync(int id, int userId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null || booking.UserId != userId)
            {
                return false;
            }

            if (booking.Status == "Completed" || booking.Status == "Cancelled")
            {
                return false; // Cannot cancel completed or already cancelled bookings
            }

            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int providerId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            return await _unitOfWork.Bookings.IsTimeSlotAvailableAsync(providerId, bookingDate, startTime, endTime);
        }

        public async Task<PaymentDto> ProcessPaymentAsync(int bookingId, string paymentMethod)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found");
            }

            return await _paymentService.ProcessPaymentAsync(bookingId, paymentMethod, booking.TotalAmount);
        }
    }
}
