using ServiceManagementSystem.Core.Entities;

namespace ServiceManagementSystem.Core.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task<IEnumerable<Service>> GetServicesByProviderAsync(int providerId);
        Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category);
        Task<IEnumerable<Service>> GetActiveServicesAsync();
        Task<IEnumerable<Service>> SearchServicesAsync(string searchTerm);
    }
}
