using Microsoft.AspNetCore.Mvc;
using ServiceManagementSystem.Core.DTOs;
using ServiceManagementSystem.Core.Interfaces;

namespace ServiceManagementSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var bookings = await _bookingService.GetBookingsByStatusAsync("All");
            return Ok(bookings);
        }

        /// <summary>
        /// Get booking by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        /// <summary>
        /// Get bookings by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByUser(int userId)
        {
            var bookings = await _bookingService.GetBookingsByUserAsync(userId);
            return Ok(bookings);
        }

        /// <summary>
        /// Get bookings by provider ID
        /// </summary>
        [HttpGet("provider/{providerId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByProvider(int providerId)
        {
            var bookings = await _bookingService.GetBookingsByProviderAsync(providerId);
            return Ok(bookings);
        }

        /// <summary>
        /// Get bookings by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByStatus(string status)
        {
            var bookings = await _bookingService.GetBookingsByStatusAsync(status);
            return Ok(bookings);
        }

        /// <summary>
        /// Get bookings by date range
        /// </summary>
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var bookings = await _bookingService.GetBookingsByDateRangeAsync(startDate, endDate);
            return Ok(bookings);
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto createBookingDto)
        {
            try
            {
                // TODO: Get user ID from JWT token
                var userId = 1; // Placeholder - should come from authenticated user
                
                var booking = await _bookingService.CreateBookingAsync(createBookingDto, userId);
                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update booking status
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<BookingDto>> UpdateBookingStatus(int id, UpdateBookingStatusDto updateDto)
        {
            try
            {
                var booking = await _bookingService.UpdateBookingStatusAsync(id, updateDto);
                return Ok(booking);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancel a booking
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                // TODO: Get user ID from JWT token
                var userId = 1; // Placeholder - should come from authenticated user
                
                var result = await _bookingService.CancelBookingAsync(id, userId);
                if (!result)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Process payment for a booking
        /// </summary>
        [HttpPost("{id}/payment")]
        public async Task<ActionResult<PaymentDto>> ProcessPayment(int id, [FromBody] string paymentMethod)
        {
            try
            {
                var payment = await _bookingService.ProcessPaymentAsync(id, paymentMethod);
                return Ok(payment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Check if time slot is available
        /// </summary>
        [HttpGet("availability")]
        public async Task<ActionResult<bool>> CheckAvailability(
            [FromQuery] int providerId,
            [FromQuery] DateTime bookingDate,
            [FromQuery] TimeSpan startTime,
            [FromQuery] TimeSpan endTime)
        {
            var isAvailable = await _bookingService.IsTimeSlotAvailableAsync(providerId, bookingDate, startTime, endTime);
            return Ok(isAvailable);
        }
    }
}
