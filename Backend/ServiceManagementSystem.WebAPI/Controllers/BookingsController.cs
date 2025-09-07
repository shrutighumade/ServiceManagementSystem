using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServiceManagementSystem.Core.DTOs;
using ServiceManagementSystem.Core.Interfaces;
using System.Security.Claims;

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
        [Authorize]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto createBookingDto)
        {
            try
            {
                // Log the incoming request
                Console.WriteLine($"CreateBooking request: ServiceId={createBookingDto.ServiceId}, Date={createBookingDto.BookingDate}, Time={createBookingDto.StartTime}, Address={createBookingDto.Address}");

                // Get user ID from JWT token claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    Console.WriteLine("Invalid user token - no user ID claim found");
                    return Unauthorized("Invalid user token");
                }

                Console.WriteLine($"User ID from token: {userId}");

                var booking = await _bookingService.CreateBookingAsync(createBookingDto, userId);
                Console.WriteLine($"Booking created successfully: ID={booking.Id}");
                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"KeyNotFoundException: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"InvalidOperationException: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
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
