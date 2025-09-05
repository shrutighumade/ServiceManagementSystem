using Microsoft.EntityFrameworkCore;
using ServiceManagementSystem.Core.Entities;
using ServiceManagementSystem.Core.Interfaces;
using ServiceManagementSystem.Infrastructure.Data;

namespace ServiceManagementSystem.Infrastructure.Repositories
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Service>> GetServicesByProviderAsync(int providerId)
        {
            return await _dbSet
                .Include(s => s.Provider)
                .ThenInclude(p => p.User)
                .Where(s => s.ProviderId == providerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category)
        {
            return await _dbSet
                .Include(s => s.Provider)
                .ThenInclude(p => p.User)
                .Where(s => s.Category == category && s.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        {
            return await _dbSet
                .Include(s => s.Provider)
                .ThenInclude(p => p.User)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> SearchServicesAsync(string searchTerm)
        {
            return await _dbSet
                .Include(s => s.Provider)
                .ThenInclude(p => p.User)
                .Where(s => s.IsActive && 
                    (s.Name.Contains(searchTerm) || 
                     s.Description.Contains(searchTerm) || 
                     s.Category.Contains(searchTerm)))
                .ToListAsync();
        }
    }
}
