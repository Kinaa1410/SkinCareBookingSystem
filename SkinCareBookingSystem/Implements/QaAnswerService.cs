using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class QaAnswerService : IQaAnswerService
    {
        private readonly BookingDbContext _context;

        public QaAnswerService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QaAnswerDTO>> GetAllQaAnswersAsync()
        {
            return await _context.QaAnswers
                .Select(answer => new QaAnswerDTO
                {
                    UserId = answer.UserId,
                    QaId = answer.QaId,
                    QaOptionId = answer.QaOptionId
                }).ToListAsync();
        }

        public async Task<QaAnswerDTO> GetQaAnswerByIdsAsync(int userId, int qaId)
        {
            var qaAnswer = await _context.QaAnswers
                .FirstOrDefaultAsync(a => a.UserId == userId && a.QaId == qaId);
            if (qaAnswer == null) return null;

            return new QaAnswerDTO
            {
                UserId = qaAnswer.UserId,
                QaId = qaAnswer.QaId,
                QaOptionId = qaAnswer.QaOptionId
            };
        }

        public async Task<QaAnswerDTO> CreateQaAnswerAsync(CreateQaAnswerDTO qaAnswerDTO)
        {
            var qaAnswer = new QaAnswer
            {
                UserId = qaAnswerDTO.UserId,
                QaId = qaAnswerDTO.QaId,
                QaOptionId = qaAnswerDTO.QaOptionId
            };

            _context.QaAnswers.Add(qaAnswer);
            await _context.SaveChangesAsync();

            return new QaAnswerDTO
            {
                UserId = qaAnswer.UserId,
                QaId = qaAnswer.QaId,
                QaOptionId = qaAnswer.QaOptionId
            };
        }

        public async Task<bool> UpdateQaAnswerAsync(int userId, int qaId, UpdateQaAnswerDTO qaAnswerDTO)
        {
            var qaAnswer = await _context.QaAnswers
                .FirstOrDefaultAsync(a => a.UserId == userId && a.QaId == qaId);
            if (qaAnswer == null) return false;

            qaAnswer.QaOptionId = qaAnswerDTO.QaOptionId;

            _context.Entry(qaAnswer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteQaAnswerAsync(int userId, int qaId)
        {
            var qaAnswer = await _context.QaAnswers
                .FirstOrDefaultAsync(a => a.UserId == userId && a.QaId == qaId);
            if (qaAnswer == null) return false;

            _context.QaAnswers.Remove(qaAnswer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ServiceDTO>> SubmitAnswersAndGetRecommendationsAsync(List<CreateQaAnswerDTO> qaAnswersDTO)
        {
            if (qaAnswersDTO == null || !qaAnswersDTO.Any())
                return new List<ServiceDTO>();
            foreach (var dto in qaAnswersDTO)
            {
                var existingAnswer = await _context.QaAnswers
                    .FirstOrDefaultAsync(a => a.UserId == dto.UserId && a.QaId == dto.QaId);
                if (existingAnswer != null)
                {
                    existingAnswer.QaOptionId = dto.QaOptionId;
                    _context.Entry(existingAnswer).State = EntityState.Modified;
                }
                else
                {
                    _context.QaAnswers.Add(new QaAnswer
                    {
                        UserId = dto.UserId,
                        QaId = dto.QaId,
                        QaOptionId = dto.QaOptionId
                    });
                }
            }
            await _context.SaveChangesAsync();

            var qaOptionIds = qaAnswersDTO.Select(dto => dto.QaOptionId).ToList();
            var serviceCounts = await _context.QaOptionServices
                .Where(qos => qaOptionIds.Contains(qos.QaOptionId))
                .GroupBy(qos => qos.ServiceId)
                .Select(g => new { ServiceId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            var serviceIds = serviceCounts.Select(sc => sc.ServiceId).ToList();
            var services = await _context.Services
                .Where(s => serviceIds.Contains(s.ServiceId))
                .Select(s => new ServiceDTO
                {
                    ServiceId = s.ServiceId,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    Rating = s.Rating,
                    Status = s.Status,
                    Exist = s.Exist,
                    ServiceCategoryId = s.ServiceCategoryId
                })
                .ToListAsync();

            return services.OrderBy(s => serviceCounts.First(sc => sc.ServiceId == s.ServiceId).Count);
        }
    }
}