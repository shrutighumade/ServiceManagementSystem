using Microsoft.EntityFrameworkCore;
using ServiceManagementSystem.Core.Entities;

namespace ServiceManagementSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ProviderAvailability> ProviderAvailabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.HasOne(e => e.Provider)
                    .WithOne(e => e.User)
                    .HasForeignKey<Provider>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Provider configuration
            modelBuilder.Entity<Provider>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BusinessName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                
                entity.Property(e => e.Rating).HasPrecision(3, 2);
                
                entity.HasOne(e => e.User)
                    .WithOne(e => e.Provider)
                    .HasForeignKey<Provider>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Service configuration
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasPrecision(10, 2);
                
                entity.HasOne(e => e.Provider)
                    .WithMany(e => e.Services)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Booking configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.SpecialInstructions).HasMaxLength(1000);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
                
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Provider)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Service)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Payment)
                    .WithOne(e => e.Booking)
                    .HasForeignKey<Payment>(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Payment configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FailureReason).HasMaxLength(500);
                entity.Property(e => e.Amount).HasPrecision(10, 2);
                
                entity.HasIndex(e => e.TransactionId).IsUnique();
            });

            // ProviderAvailability configuration
            modelBuilder.Entity<ProviderAvailability>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Provider)
                    .WithMany(e => e.Availabilities)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@servicemanagement.com",
                    PhoneNumber = "1234567890",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed sample provider user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Provider",
                    Email = "provider@example.com",
                    PhoneNumber = "0987654321",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Provider123!"),
                    Role = "Provider",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed sample customer user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 3,
                    FirstName = "Jane",
                    LastName = "Customer",
                    Email = "customer@example.com",
                    PhoneNumber = "1122334455",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed sample provider
            modelBuilder.Entity<Provider>().HasData(
                new Provider
                {
                    Id = 1,
                    UserId = 2,
                    BusinessName = "CleanPro Services",
                    Description = "Professional cleaning services for homes and offices",
                    Address = "123 Main Street",
                    City = "New York",
                    PostalCode = "10001",
                    Country = "USA",
                    Rating = 4.5m,
                    TotalReviews = 25,
                    IsVerified = true,
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed sample services
            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = 1,
                    ProviderId = 1,
                    Name = "House Cleaning",
                    Description = "Complete house cleaning including all rooms, kitchen, and bathrooms",
                    Category = "Cleaning",
                    Price = 150.00m,
                    DurationMinutes = 180,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Id = 2,
                    ProviderId = 1,
                    Name = "Office Cleaning",
                    Description = "Professional office cleaning services",
                    Category = "Cleaning",
                    Price = 200.00m,
                    DurationMinutes = 240,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
