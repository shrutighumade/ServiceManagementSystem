using AutoMapper;
using ServiceManagementSystem.Core.DTOs;
using ServiceManagementSystem.Core.Entities;
using ServiceManagementSystem.Core.Interfaces;

namespace ServiceManagementSystem.Infrastructure.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceDto?> GetServiceByIdAsync(int id)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(id);
            return service != null ? _mapper.Map<ServiceDto>(service) : null;
        }

        public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
        {
            var services = await _unitOfWork.Services.GetActiveServicesAsync();
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<IEnumerable<ServiceDto>> GetServicesByProviderAsync(int providerId)
        {
            var services = await _unitOfWork.Services.GetServicesByProviderAsync(providerId);
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<IEnumerable<ServiceDto>> GetServicesByCategoryAsync(string category)
        {
            var services = await _unitOfWork.Services.GetServicesByCategoryAsync(category);
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<IEnumerable<ServiceDto>> SearchServicesAsync(string searchTerm)
        {
            var services = await _unitOfWork.Services.SearchServicesAsync(searchTerm);
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto createServiceDto)
        {
            var service = _mapper.Map<Service>(createServiceDto);
            service.IsActive = true;
            service.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ServiceDto>(service);
        }

        public async Task<ServiceDto> UpdateServiceAsync(int id, UpdateServiceDto updateServiceDto)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(id);
            if (service == null)
            {
                throw new KeyNotFoundException("Service not found");
            }

            _mapper.Map(updateServiceDto, service);
            service.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Services.UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ServiceDto>(service);
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(id);
            if (service == null)
            {
                return false;
            }

            // Soft delete by setting IsActive to false
            service.IsActive = false;
            service.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Services.UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetServiceCategoriesAsync()
        {
            var services = await _unitOfWork.Services.GetActiveServicesAsync();
            return services.Select(s => s.Category).Distinct().OrderBy(c => c);
        }
    }
}
