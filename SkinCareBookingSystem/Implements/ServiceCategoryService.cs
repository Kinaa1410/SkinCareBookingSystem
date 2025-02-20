using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class ServiceCategoryService : IServiceCategoryService
    {
        private readonly BookingDbContext _context;

        public ServiceCategoryService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceCategoryDTO>> GetAllServiceCategoriesAsync()
        {
            return await _context.ServiceCategories
                .Select(sc => new ServiceCategoryDTO
                {
                    ServiceCategoryId = sc.ServiceCategoryId,
                    Name = sc.Name,
                    Status = sc.Status,
                    Exist = sc.Exist
                }).ToListAsync();
        }

        public async Task<ServiceCategoryDTO> GetServiceCategoryByIdAsync(int serviceCategoryId)
        {
            var serviceCategory = await _context.ServiceCategories
                .FirstOrDefaultAsync(sc => sc.ServiceCategoryId == serviceCategoryId);
            if (serviceCategory == null) return null;

            return new ServiceCategoryDTO
            {
                ServiceCategoryId = serviceCategory.ServiceCategoryId,
                Name = serviceCategory.Name,
                Status = serviceCategory.Status,
                Exist = serviceCategory.Exist
            };
        }

        public async Task<ServiceCategoryDTO> CreateServiceCategoryAsync(CreateServiceCategoryDTO serviceCategoryDTO)
        {
            var serviceCategory = new ServiceCategory
            {
                Name = serviceCategoryDTO.Name,
                Status = serviceCategoryDTO.Status,
                Exist = serviceCategoryDTO.Exist
            };

            _context.ServiceCategories.Add(serviceCategory);
            await _context.SaveChangesAsync();

            return new ServiceCategoryDTO
            {
                ServiceCategoryId = serviceCategory.ServiceCategoryId,
                Name = serviceCategory.Name,
                Status = serviceCategory.Status,
                Exist = serviceCategory.Exist
            };
        }

        public async Task<bool> UpdateServiceCategoryAsync(int serviceCategoryId, UpdateServiceCategoryDTO serviceCategoryDTO)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(serviceCategoryId);
            if (serviceCategory == null) return false;

            serviceCategory.Name = serviceCategoryDTO.Name;
            serviceCategory.Status = serviceCategoryDTO.Status;
            serviceCategory.Exist = serviceCategoryDTO.Exist;

            _context.Entry(serviceCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceCategoryAsync(int serviceCategoryId)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(serviceCategoryId);
            if (serviceCategory == null) return false;

            _context.ServiceCategories.Remove(serviceCategory);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
