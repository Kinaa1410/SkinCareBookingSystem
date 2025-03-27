using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class QaService : IQaService
    {
        private readonly BookingDbContext _context;

        public QaService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QaDTO>> GetAllQasAsync()
        {
            return await _context.Qas
                .Select(qa => new QaDTO
                {
                    QaId = qa.QaId,
                    ServiceCategoryId = qa.ServiceCategoryId,
                    Question = qa.Question,
                    Type = qa.Type,
                    Status = qa.Status
                }).ToListAsync();
        }

        public async Task<QaDTO> GetQaByIdAsync(int qaId)
        {
            var qa = await _context.Qas
                .FirstOrDefaultAsync(q => q.QaId == qaId);
            if (qa == null) return null;

            return new QaDTO
            {
                QaId = qa.QaId,
                ServiceCategoryId = qa.ServiceCategoryId,
                Question = qa.Question,
                Type = qa.Type,
                Status = qa.Status
            };
        }

        public async Task<QaDTO> CreateQaAsync(CreateQaDTO qaDTO)
        {
            var qa = new Qa
            {
                ServiceCategoryId = qaDTO.ServiceCategoryId,
                Question = qaDTO.Question,
                Type = qaDTO.Type,
                Status = qaDTO.Status
            };

            _context.Qas.Add(qa);
            await _context.SaveChangesAsync();

            return new QaDTO
            {
                QaId = qa.QaId,
                ServiceCategoryId = qa.ServiceCategoryId,
                Question = qa.Question,
                Type = qa.Type,
                Status = qa.Status
            };
        }

        public async Task<bool> UpdateQaAsync(int qaId, UpdateQaDTO qaDTO)
        {
            var qa = await _context.Qas.FindAsync(qaId);
            if (qa == null) return false;

            qa.Question = qaDTO.Question;
            qa.Type = qaDTO.Type;
            qa.Status = qaDTO.Status;

            _context.Entry(qa).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteQaAsync(int qaId)
        {
            var qa = await _context.Qas.FindAsync(qaId);
            if (qa == null) return false;

            _context.Qas.Remove(qa);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<QaDTO>> GetQasByServiceCategoryAsync(int serviceCategoryId)
        {
            var results = await _context.Qas
                .Where(qa => qa.ServiceCategoryId == serviceCategoryId)
                .Select(qa => new QaDTO
                {
                    QaId = qa.QaId,
                    ServiceCategoryId = qa.ServiceCategoryId,
                    Question = qa.Question,
                    Type = qa.Type,
                    Status = qa.Status
                })
                .ToListAsync();

            return results;
        }
    }
}
