# Service Management System

A comprehensive .NET-based service management platform built with Clean Architecture, featuring real-time notifications, JWT authentication, and a modern web interface.

## 🚀 Quick Start - How to Run

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code (optional)

### 1. Clone and Navigate

```bash
git clone https://github.com/shrutighumade/ServiceManagementSystem
cd DotNetProject
```


## 2. Restore Dependencies

Before restoring, make sure the official NuGet feed is configured:

dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org  
dotnet nuget list source  

You should see nuget.org in the list of registered sources.

Now restore all project dependencies:

dotnet restore  

---

### 3. Run the WebAPI (Backend)

```bash
dotnet run --project Backend\ServiceManagementSystem.WebAPI
```

- **API URL**: `http://localhost:5107`
- **Swagger UI**: `http://localhost:5107/swagger`

### 4. Run the MVC Frontend (Optional)

```bash
dotnet run --project Frontend\ServiceManagementSystem.MVC
```

- **Frontend URL**: `http://localhost:5107`

### 5. Test the API

Use the provided `test-api.http` file or visit the Swagger UI to test endpoints.

## 📋 Project Structure

```
ServiceManagementSystem/
├── Backend/
│   ├── ServiceManagementSystem.Core/          # Domain entities, DTOs, interfaces
│   ├── ServiceManagementSystem.Infrastructure/ # Data access, repositories, services
│   └── ServiceManagementSystem.WebAPI/        # REST API controllers
├── Frontend/
│   └── ServiceManagementSystem.MVC/           # Web UI (Razor + Bootstrap)
└── test-api.http                              # API testing file
```

## 🏗️ Architecture

### Clean Architecture Layers

- **Core**: Domain entities, DTOs, business interfaces
- **Infrastructure**: Data access, external services, repository implementations
- **WebAPI**: REST API controllers, middleware, configuration
- **MVC**: Web UI consuming the API

### Key Technologies

- **Backend**: ASP.NET Core Web API, Entity Framework Core, SQLite
- **Frontend**: ASP.NET Core MVC, Bootstrap, Razor Pages
- **Authentication**: JWT Bearer tokens with role-based authorization
- **Real-time**: SignalR for live notifications
- **Database**: SQLite (file-based, no installation required)
- **Mapping**: AutoMapper for DTO transformations
- **Documentation**: Swagger/OpenAPI

## 🎯 Features

### ✅ Implemented Features

- **User Management**: Registration, login, role-based access (User, Provider, Admin)
- **Service Management**: CRUD operations for services with categories
- **Booking System**: Create, update, cancel bookings with time slot validation
- **Payment Processing**: Dummy payment gateway with random success/failure
- **Real-time Notifications**: SignalR for booking updates
- **API Documentation**: Swagger UI for testing and documentation
- **Database**: SQLite with seed data (Admin, Provider, Customer users)

### 🔄 In Progress

- JWT Authentication implementation
- Advanced reporting features
- Unit and integration tests

## 📊 Database Schema

### Core Entities

- **User**: Authentication and profile information
- **Provider**: Service provider details and business information
- **Service**: Available services with pricing and duration
- **Booking**: Service bookings with status tracking
- **Payment**: Payment processing and transaction history
- **ProviderAvailability**: Time slot management

### Seed Data

The system comes with pre-configured users:

- **Admin**: `admin@servicemanagement.com` / `Admin123!`
- **Provider**: `provider@example.com` / `Provider123!`
- **Customer**: `customer@example.com` / `Customer123!`

## 🔌 API Endpoints

### Authentication

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/user/{id}` - Get user details

### Services

- `GET /api/services` - Get all services
- `GET /api/services/{id}` - Get service by ID
- `GET /api/services/category/{category}` - Get services by category
- `GET /api/services/search?term={term}` - Search services
- `POST /api/services` - Create new service
- `PUT /api/services/{id}` - Update service
- `DELETE /api/services/{id}` - Delete service

### Bookings

- `GET /api/bookings` - Get all bookings
- `GET /api/bookings/user/{userId}` - Get user bookings
- `GET /api/bookings/provider/{providerId}` - Get provider bookings
- `POST /api/bookings` - Create new booking
- `PUT /api/bookings/{id}/status` - Update booking status
- `PUT /api/bookings/{id}/cancel` - Cancel booking
- `POST /api/bookings/{id}/payment` - Process payment

## 🛠️ Development

### Adding New Features

1. **Domain Layer**: Add entities and DTOs in Core project
2. **Data Layer**: Implement repositories in Infrastructure project
3. **Business Layer**: Create services in Infrastructure project
4. **API Layer**: Add controllers in WebAPI project
5. **UI Layer**: Create views and controllers in MVC project

### Database Migrations

```bash
# Add migration
dotnet ef migrations add MigrationName --project Backend\ServiceManagementSystem.Infrastructure --startup-project Backend\ServiceManagementSystem.WebAPI

# Update database
dotnet ef database update --project Backend\ServiceManagementSystem.Infrastructure --startup-project Backend\ServiceManagementSystem.WebAPI
```

### Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Backend\ServiceManagementSystem.Tests
```

## 🔧 Configuration

### Connection Strings

The system uses SQLite by default. To change to SQL Server:

1. Update `appsettings.json` connection string
2. Add SQL Server package: `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
3. Update `ServiceCollectionExtensions.cs` to use `UseSqlServer()`

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Set to `Development` for detailed errors
- `ConnectionStrings__DefaultConnection`: Override database connection

## 📝 API Testing

### Using Swagger UI

1. Run the WebAPI project
2. Navigate to `http://localhost:5107/swagger`
3. Use the interactive API documentation

### Using HTTP Files

1. Open `test-api.http` in VS Code with REST Client extension
2. Run individual requests or the entire file

### Sample API Calls

```bash
# Get all services
curl -X GET "http://localhost:5107/api/services"

# Login
curl -X POST "http://localhost:5107/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@servicemanagement.com","password":"Admin123!"}'

# Create booking
curl -X POST "http://localhost:5107/api/bookings" \
  -H "Content-Type: application/json" \
  -d '{"serviceId":1,"bookingDate":"2024-01-15T00:00:00Z","startTime":"09:00:00","address":"123 Test Street"}'
```

## 🚨 Troubleshooting

### Common Issues

**Database Connection Error**

- Ensure SQLite database file is created
- Check connection string in `appsettings.json`

**Build Errors**

- Run `dotnet restore` to restore packages
- Check for missing package references

**Port Conflicts**

- Update ports in `launchSettings.json`
- Use `dotnet run --urls "https://localhost:5001"`

**CORS Issues**

- Verify CORS policy in `Program.cs`
- Check frontend URL configuration

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [JWT Authentication](https://jwt.io/)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**Happy Coding! 🎉**

For questions or support, please open an issue in the repository.
