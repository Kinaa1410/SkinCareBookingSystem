using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IQaOptionService
    {
        Task<IEnumerable<QaOptionDTO>> GetAllQaOptionsAsync(int qaId);
        Task<QaOptionDTO> GetQaOptionByIdAsync(int qaOptionId);
        Task<QaOptionDTO> CreateQaOptionAsync(int qaId, CreateQaOptionDTO qaOptionDTO);
        Task<bool> UpdateQaOptionAsync(int qaOptionId, UpdateQaOptionDTO qaOptionDTO);
        Task<bool> DeleteQaOptionAsync(int qaOptionId);
    }
}