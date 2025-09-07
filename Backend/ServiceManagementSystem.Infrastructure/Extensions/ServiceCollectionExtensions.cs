using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using ServiceManagementSystem.Core.Interfaces;
using ServiceManagementSystem.Infrastructure.Data;
using ServiceManagementSystem.Infrastructure.Repositories;
using ServiceManagementSystem.Infrastructure.Services;

namespace ServiceManagementSystem.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            // Services
            services.AddScoped<IUserService>(provider =>
            {
                var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
                var mapper = provider.GetRequiredService<IMapper>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                return new UserService(unitOfWork, mapper, configuration);
            });
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
