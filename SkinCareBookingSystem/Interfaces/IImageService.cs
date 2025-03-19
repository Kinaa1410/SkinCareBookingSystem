using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IImageService
    {
        Task<ImageServiceDTO> CreateImageServiceAsync(CreateImageServiceDTO imageServiceDTO);
        Task<ImageServiceDTO> UpdateImageServiceAsync(int imageServiceId, UpdateImageServiceDTO imageServiceDTO);
        Task<bool> DeleteImageServiceAsync(int imageServiceId);
        Task<ImageServiceDTO> GetImageServiceAsync(int imageServiceId);
        Task<IEnumerable<ImageServiceDTO>> GetImageServiceByServiceIdAsync(int serviceId);
    }
}
