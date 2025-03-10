using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class ServiceRecommendationService : IServiceRecommendationService
    {
        private readonly BookingDbContext _context;

        public ServiceRecommendationService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRecommendationDTO>> GetAllServiceRecommendationsAsync()
        {
            return await _context.ServiceRecommendations
                .Select(sr => new ServiceRecommendationDTO
                {
                    Id = sr.Id,
                    QaId = sr.QaId,
                    AnswerOption = sr.AnswerOption,
                    ServiceId = sr.ServiceId,
                    Weight = sr.Weight
                }).ToListAsync();
        }

        public async Task<ServiceRecommendationDTO> GetServiceRecommendationByIdAsync(int id)
        {
            var sr = await _context.ServiceRecommendations.FindAsync(id);
            if (sr == null) return null;

            return new ServiceRecommendationDTO
            {
                Id = sr.Id,
                QaId = sr.QaId,
                AnswerOption = sr.AnswerOption,
                ServiceId = sr.ServiceId,
                Weight = sr.Weight
            };
        }

        public async Task<ServiceRecommendationDTO> CreateServiceRecommendationAsync(CreateServiceRecommendationDTO dto)
        {
            var sr = new ServiceRecommendation
            {
                QaId = dto.QaId,
                AnswerOption = dto.AnswerOption,
                ServiceId = dto.ServiceId,
                Weight = dto.Weight
            };

            _context.ServiceRecommendations.Add(sr);
            await _context.SaveChangesAsync();

            return new ServiceRecommendationDTO
            {
                Id = sr.Id,
                QaId = sr.QaId,
                AnswerOption = sr.AnswerOption,
                ServiceId = sr.ServiceId,
                Weight = sr.Weight
            };
        }

        public async Task<bool> UpdateServiceRecommendationAsync(int id, UpdateServiceRecommendationDTO dto)
        {
            var sr = await _context.ServiceRecommendations.FindAsync(id);
            if (sr == null) return false;

            sr.Weight = dto.Weight;

            _context.Entry(sr).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceRecommendationAsync(int id)
        {
            var sr = await _context.ServiceRecommendations.FindAsync(id);
            if (sr == null) return false;

            _context.ServiceRecommendations.Remove(sr);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ServiceDTO>> GetRecommendedServicesAsync(List<QaAnswerDTO> answers)
        {
            var serviceRecommendations = await _context.ServiceRecommendations.ToListAsync();

            var recommendedServiceIds = serviceRecommendations
                .Where(sr => answers.Any(a => a.QaId == sr.QaId && a.Answer == sr.AnswerOption))
                .GroupBy(sr => sr.ServiceId)
                .Select(g => new { ServiceId = g.Key, Score = g.Sum(sr => sr.Weight) })
                .OrderByDescending(g => g.Score)
                .Select(g => g.ServiceId)
                .ToList();
            if (!recommendedServiceIds.Any())
            {
                return new List<ServiceDTO>();
            }
            var recommendedServices = await _context.Services
                .Where(s => recommendedServiceIds.Contains(s.ServiceId))
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
            return recommendedServices;
        }


    }
}
