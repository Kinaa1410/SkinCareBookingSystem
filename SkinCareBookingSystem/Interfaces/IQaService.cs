using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IQaService
    {
        Task<IEnumerable<QaDTO>> GetAllQasAsync();
        Task<QaDTO> GetQaByIdAsync(int qaId);
        Task<QaDTO> CreateQaAsync(CreateQaDTO qaDTO);
        Task<bool> UpdateQaAsync(int qaId, UpdateQaDTO qaDTO);
        Task<bool> DeleteQaAsync(int qaId);
    }
}
