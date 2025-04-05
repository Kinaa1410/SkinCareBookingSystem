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
                .Include(qa => qa.Options)
                    .ThenInclude(o => o.ServiceRecommendations)
                .Select(qa => new QaDTO
                {
                    QaId = qa.QaId,
                    ServiceCategoryId = qa.ServiceCategoryId,
                    Question = qa.Question,
                    Type = qa.Type,
                    Status = qa.Status,
                    Options = qa.Options.Select(o => new QaOptionDTO
                    {
                        QaOptionId = o.QaOptionId,
                        AnswerText = o.AnswerText,
                        ServiceIds = o.ServiceRecommendations.Select(sr => sr.ServiceId).ToList()
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<QaDTO> GetQaByIdAsync(int qaId)
        {
            var qa = await _context.Qas
                .Include(qa => qa.Options)
                    .ThenInclude(o => o.ServiceRecommendations)
                .FirstOrDefaultAsync(q => q.QaId == qaId);

            if (qa == null) return null;

            return new QaDTO
            {
                QaId = qa.QaId,
                ServiceCategoryId = qa.ServiceCategoryId,
                Question = qa.Question,
                Type = qa.Type,
                Status = qa.Status,
                Options = qa.Options.Select(o => new QaOptionDTO
                {
                    QaOptionId = o.QaOptionId,
                    AnswerText = o.AnswerText,
                    ServiceIds = o.ServiceRecommendations.Select(sr => sr.ServiceId).ToList()
                }).ToList()
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

            foreach (var optionDto in qaDTO.Options)
            {
                var qaOption = new QaOption
                {
                    AnswerText = optionDto.AnswerText,
                    Qa = qa
                };

                foreach (var serviceId in optionDto.ServiceIds)
                {
                    qaOption.ServiceRecommendations.Add(new QaOptionService
                    {
                        ServiceId = serviceId
                    });
                }

                qa.Options.Add(qaOption);
            }

            _context.Qas.Add(qa);
            await _context.SaveChangesAsync();

            return new QaDTO
            {
                QaId = qa.QaId,
                ServiceCategoryId = qa.ServiceCategoryId,
                Question = qa.Question,
                Type = qa.Type,
                Status = qa.Status,
                Options = qa.Options.Select(o => new QaOptionDTO
                {
                    QaOptionId = o.QaOptionId,
                    AnswerText = o.AnswerText,
                    ServiceIds = o.ServiceRecommendations.Select(sr => sr.ServiceId).ToList()
                }).ToList()
            };
        }

        public async Task<bool> UpdateQaAsync(int qaId, UpdateQaDTO qaDTO)
        {
            var qa = await _context.Qas
                .Include(q => q.Options)
                    .ThenInclude(o => o.ServiceRecommendations)
                .FirstOrDefaultAsync(q => q.QaId == qaId);

            if (qa == null) return false;

            qa.Question = qaDTO.Question;
            qa.Type = qaDTO.Type;
            qa.Status = qaDTO.Status;

            var incomingOptionIds = qaDTO.Options
                .Where(o => o.QaOptionId.HasValue)
                .Select(o => o.QaOptionId.Value)
                .ToList();

            var optionsToRemove = qa.Options
                .Where(o => !incomingOptionIds.Contains(o.QaOptionId))
                .ToList();

            foreach (var option in optionsToRemove)
            {
                _context.QaOptions.Remove(option);
            }

            foreach (var optionDto in qaDTO.Options)
            {
                if (optionDto.QaOptionId.HasValue)
                {
                    var existingOption = qa.Options.FirstOrDefault(o => o.QaOptionId == optionDto.QaOptionId.Value);
                    if (existingOption != null)
                    {
                        existingOption.AnswerText = optionDto.AnswerText;
                        _context.QaOptionServices.RemoveRange(existingOption.ServiceRecommendations);
                        existingOption.ServiceRecommendations = optionDto.ServiceIds
                            .Select(serviceId => new QaOptionService { ServiceId = serviceId })
                            .ToList();
                    }
                }
                else
                {
                    var newOption = new QaOption
                    {
                        AnswerText = optionDto.AnswerText,
                        QaId = qa.QaId,
                        ServiceRecommendations = optionDto.ServiceIds
                            .Select(serviceId => new QaOptionService { ServiceId = serviceId })
                            .ToList()
                    };
                    qa.Options.Add(newOption);
                }
            }

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
            return await _context.Qas
                .Where(qa => qa.ServiceCategoryId == serviceCategoryId)
                .Include(qa => qa.Options)
                    .ThenInclude(o => o.ServiceRecommendations)
                .Select(qa => new QaDTO
                {
                    QaId = qa.QaId,
                    ServiceCategoryId = qa.ServiceCategoryId,
                    Question = qa.Question,
                    Type = qa.Type,
                    Status = qa.Status,
                    Options = qa.Options.Select(o => new QaOptionDTO
                    {
                        QaOptionId = o.QaOptionId,
                        AnswerText = o.AnswerText,
                        ServiceIds = o.ServiceRecommendations.Select(sr => sr.ServiceId).ToList()
                    }).ToList()
                }).ToListAsync();
        }
    }
}