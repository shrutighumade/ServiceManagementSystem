using Microsoft.AspNetCore.Mvc;
using ServiceManagementSystem.MVC.Models;
using ServiceManagementSystem.MVC.Services;

namespace ServiceManagementSystem.MVC.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<ServicesController> _logger;

        public ServicesController(ApiService apiService, ILogger<ServicesController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? category)
        {
            try
            {
                var services = await _apiService.GetServicesAsync();
                var categories = await _apiService.GetServiceCategoriesAsync();

                if (!string.IsNullOrEmpty(category))
                {
                    services = services.Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                ViewBag.Categories = categories;
                ViewBag.SelectedCategory = category;

                return View(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading services");
                TempData["ErrorMessage"] = "An error occurred while loading services.";
                return View(new List<ServiceViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var service = await _apiService.GetServiceAsync(id);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToAction("Index");
                }

                return View(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading service details");
                TempData["ErrorMessage"] = "An error occurred while loading service details.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {
            try
            {
                var service = await _apiService.GetServiceAsync(id);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToAction("Index");
                }

                var model = new CreateBookingViewModel
                {
                    ServiceId = id
                };

                ViewBag.Service = service;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading booking form");
                TempData["ErrorMessage"] = "An error occurred while loading the booking form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Book(CreateBookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var service = await _apiService.GetServiceAsync(model.ServiceId);
                ViewBag.Service = service;
                return View(model);
            }

            try
            {
                var booking = await _apiService.CreateBookingAsync(model);
                if (booking != null)
                {
                    TempData["SuccessMessage"] = "Booking created successfully!";
                    return RedirectToAction("Index", "Bookings");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create booking. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                ModelState.AddModelError("", "An error occurred while creating the booking. Please try again.");
            }

            var serviceForView = await _apiService.GetServiceAsync(model.ServiceId);
            ViewBag.Service = serviceForView;
            return View(model);
        }
    }
}
