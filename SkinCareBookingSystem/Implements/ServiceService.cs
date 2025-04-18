﻿using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class ServiceService : IServiceService
    {
        private readonly BookingDbContext _context;

        public ServiceService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceDTO>> GetAllServicesAsync()
        {
            return await _context.Services
                .Include(s => s.ServiceCategory)
                .Select(s => new ServiceDTO
                {
                    ServiceId = s.ServiceId,
                    ServiceCategoryId = s.ServiceCategoryId,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    Rating = s.Rating,
                    Status = s.Status,
                    Exist = s.Exist
                }).ToListAsync();
        }

        public async Task<ServiceDTO> GetServiceByIdAsync(int serviceId)
        {
            var service = await _context.Services
                .Include(s => s.ServiceCategory)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            if (service == null) return null;

            return new ServiceDTO
            {
                ServiceId = service.ServiceId,
                ServiceCategoryId = service.ServiceCategoryId,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price, 
                Rating = service.Rating,
                Status = service.Status,
                Exist = service.Exist
            };
        }

        public async Task<ServiceDTO> CreateServiceAsync(CreateServiceDTO serviceDTO)
        {
            if (_context.Services.Any(s => s.Name == serviceDTO.Name))
                throw new Exception("Service with this name already exists");
            var service = new Service
            {
                ServiceCategoryId = serviceDTO.ServiceCategoryId,
                Name = serviceDTO.Name,
                Description = serviceDTO.Description,
                Price = serviceDTO.Price,
                Rating = 0,
                Status = serviceDTO.Status,
                Exist = serviceDTO.Exist
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return new ServiceDTO
            {
                ServiceId = service.ServiceId,
                ServiceCategoryId = service.ServiceCategoryId,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                Rating = service.Rating,
                Status = service.Status,
                Exist = service.Exist
            };
        }

        public async Task<bool> UpdateServiceAsync(int serviceId, UpdateServiceDTO serviceDTO)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null) return false;

            service.ServiceCategoryId = serviceDTO.ServiceCategoryId;
            service.Name = serviceDTO.Name;
            service.Description = serviceDTO.Description;
            service.Price = serviceDTO.Price;
            service.Rating = serviceDTO.Rating;
            service.Status = serviceDTO.Status;
            service.Exist = serviceDTO.Exist;

            _context.Entry(service).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceAsync(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null) return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ServiceDTO>> GetAllServicesByCategoryIdAsync(int serviceCategoryId)
        {
            return await _context.Services
                .Where(s => s.ServiceCategoryId == serviceCategoryId)
                .Include(s => s.ServiceCategory)
                .Select(s => new ServiceDTO
                {
                    ServiceId = s.ServiceId,
                    ServiceCategoryId = s.ServiceCategoryId,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    Rating = s.Rating,
                    Status = s.Status,
                    Exist = s.Exist
                })
                .ToListAsync();
        }
      

      
    }
}
