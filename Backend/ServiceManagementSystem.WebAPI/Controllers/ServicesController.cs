using Microsoft.AspNetCore.Mvc;
using ServiceManagementSystem.Core.DTOs;
using ServiceManagementSystem.Core.Interfaces;

namespace ServiceManagementSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        /// <summary>
        /// Get all active services
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }

        /// <summary>
        /// Get service by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDto>> GetService(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return Ok(service);
        }

        /// <summary>
        /// Get services by provider ID
        /// </summary>
        [HttpGet("provider/{providerId}")]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServicesByProvider(int providerId)
        {
            var services = await _serviceService.GetServicesByProviderAsync(providerId);
            return Ok(services);
        }

        /// <summary>
        /// Get services by category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServicesByCategory(string category)
        {
            var services = await _serviceService.GetServicesByCategoryAsync(category);
            return Ok(services);
        }

        /// <summary>
        /// Search services
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> SearchServices([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term is required");
            }

            var services = await _serviceService.SearchServicesAsync(term);
            return Ok(services);
        }

        /// <summary>
        /// Get all service categories
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _serviceService.GetServiceCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Create a new service
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ServiceDto>> CreateService(CreateServiceDto createServiceDto)
        {
            try
            {
                var service = await _serviceService.CreateServiceAsync(createServiceDto);
                return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update a service
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, UpdateServiceDto updateServiceDto)
        {
            try
            {
                var service = await _serviceService.UpdateServiceAsync(id, updateServiceDto);
                return Ok(service);
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
        /// Delete a service (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _serviceService.DeleteServiceAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
