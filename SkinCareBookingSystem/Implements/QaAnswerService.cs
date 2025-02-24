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
                    Answer = answer.Answer
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
                Answer = qaAnswer.Answer
            };
        }

        public async Task<QaAnswerDTO> CreateQaAnswerAsync(CreateQaAnswerDTO qaAnswerDTO)
        {
            var qaAnswer = new QaAnswer
            {
                UserId = qaAnswerDTO.UserId,
                QaId = qaAnswerDTO.QaId,
                Answer = qaAnswerDTO.Answer
            };

            _context.QaAnswers.Add(qaAnswer);
            await _context.SaveChangesAsync();

            return new QaAnswerDTO
            {
                UserId = qaAnswer.UserId,
                QaId = qaAnswer.QaId,
                Answer = qaAnswer.Answer
            };
        }

        public async Task<bool> UpdateQaAnswerAsync(int userId, int qaId, UpdateQaAnswerDTO qaAnswerDTO)
        {
            var qaAnswer = await _context.QaAnswers
                .FirstOrDefaultAsync(a => a.UserId == userId && a.QaId == qaId);
            if (qaAnswer == null) return false;

            qaAnswer.Answer = qaAnswerDTO.Answer;

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
    }
}
