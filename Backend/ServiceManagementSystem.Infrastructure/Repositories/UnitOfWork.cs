using Microsoft.EntityFrameworkCore.Storage;
using ServiceManagementSystem.Core.Interfaces;
using ServiceManagementSystem.Infrastructure.Data;

namespace ServiceManagementSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Services = new ServiceRepository(_context);
            Bookings = new BookingRepository(_context);
            Providers = new Repository<Core.Entities.Provider>(_context);
            Payments = new Repository<Core.Entities.Payment>(_context);
            ProviderAvailabilities = new Repository<Core.Entities.ProviderAvailability>(_context);
        }

        public IUserRepository Users { get; }
        public IServiceRepository Services { get; }
        public IBookingRepository Bookings { get; }
        public IRepository<Core.Entities.Provider> Providers { get; }
        public IRepository<Core.Entities.Payment> Payments { get; }
        public IRepository<Core.Entities.ProviderAvailability> ProviderAvailabilities { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
