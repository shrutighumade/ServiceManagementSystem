using System.Text;
using System.Text.Json;
using ServiceManagementSystem.MVC.Models;

namespace ServiceManagementSystem.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            
            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5101";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            Console.WriteLine($"AddAuthHeader: Token present: {!string.IsNullOrEmpty(token)}");
            if (!string.IsNullOrEmpty(token))
            {
                // Remove existing Authorization header to avoid duplicates
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine("AddAuthHeader: Authorization header set");
            }
            else
            {
                // Remove Authorization header if no token
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                Console.WriteLine("AddAuthHeader: No token found, Authorization header removed");
            }
        }

        // Authentication
        public async Task<AuthResponse?> LoginAsync(LoginViewModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("/api/auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login failed: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }
            return null;
        }

        public async Task<UserViewModel?> RegisterAsync(RegisterViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/auth/register", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserViewModel>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return null;
        }

        // Services
        public async Task<List<ServiceViewModel>> GetServicesAsync()
        {
            var response = await _httpClient.GetAsync("/api/services");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ServiceViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ServiceViewModel>();
            }
            return new List<ServiceViewModel>();
        }

        public async Task<ServiceViewModel?> GetServiceAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/services/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ServiceViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return null;
        }

        public async Task<List<string>> GetServiceCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("/api/services/categories");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<string>();
            }
            return new List<string>();
        }

        // Bookings
        public async Task<List<BookingViewModel>> GetBookingsAsync()
        {
            AddAuthHeader();

            // Get user ID from session (this would be stored when user logs in)
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                // Fallback to getting all bookings if no user ID in session
                var response = await _httpClient.GetAsync("/api/bookings");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<BookingViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<BookingViewModel>();
                }
                return new List<BookingViewModel>();
            }

            // Get user-specific bookings
            var userResponse = await _httpClient.GetAsync($"/api/bookings/user/{userId}");
            if (userResponse.IsSuccessStatusCode)
            {
                var content = await userResponse.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<BookingViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<BookingViewModel>();
            }
            return new List<BookingViewModel>();
        }

        public async Task<BookingViewModel?> CreateBookingAsync(CreateBookingViewModel model)
        {
            AddAuthHeader();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/bookings", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BookingViewModel>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Booking creation failed: {response.StatusCode} - {errorContent}");
                return null;
            }
        }

        public async Task<bool> ProcessPaymentAsync(int bookingId, string paymentMethod)
        {
            AddAuthHeader();
            var json = JsonSerializer.Serialize(paymentMethod);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/bookings/{bookingId}/payment", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CheckAvailabilityAsync(int providerId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            var response = await _httpClient.GetAsync($"/api/bookings/availability?providerId={providerId}&bookingDate={bookingDate:yyyy-MM-dd}&startTime={startTime}&endTime={endTime}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return false;
        }
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserViewModel User { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
