using Microsoft.AspNetCore.Mvc;
using ServiceManagementSystem.MVC.Models;
using ServiceManagementSystem.MVC.Services;

namespace ServiceManagementSystem.MVC.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApiService _apiService;

        public BookingsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var bookings = await _apiService.GetBookingsAsync();
                return View(bookings);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading bookings.";
                return View(new List<BookingViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int bookingId, string paymentMethod = "CreditCard")
        {
            try
            {
                var success = await _apiService.ProcessPaymentAsync(bookingId, paymentMethod);
                if (success)
                {
                    TempData["SuccessMessage"] = "Payment processed successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment processing failed. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing payment.";
            }

            return RedirectToAction("Index");
        }
    }
}
