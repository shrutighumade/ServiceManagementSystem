using ServiceManagementSystem.Core.DTOs;

namespace ServiceManagementSystem.Core.Interfaces
{
    public interface IServiceService
    {
        Task<ServiceDto?> GetServiceByIdAsync(int id);
        Task<IEnumerable<ServiceDto>> GetAllServicesAsync();
        Task<IEnumerable<ServiceDto>> GetServicesByProviderAsync(int providerId);
        Task<IEnumerable<ServiceDto>> GetServicesByCategoryAsync(string category);
        Task<IEnumerable<ServiceDto>> SearchServicesAsync(string searchTerm);
        Task<ServiceDto> CreateServiceAsync(CreateServiceDto createServiceDto);
        Task<ServiceDto> UpdateServiceAsync(int id, UpdateServiceDto updateServiceDto);
        Task<bool> DeleteServiceAsync(int id);
        Task<IEnumerable<string>> GetServiceCategoriesAsync();
    }
}
