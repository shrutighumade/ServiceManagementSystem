using ServiceManagementSystem.Core.Entities;

namespace ServiceManagementSystem.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IServiceRepository Services { get; }
        IBookingRepository Bookings { get; }
        IRepository<Provider> Providers { get; }
        IRepository<Payment> Payments { get; }
        IRepository<ProviderAvailability> ProviderAvailabilities { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
