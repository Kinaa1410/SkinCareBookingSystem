using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IServiceCategoryService
    {
        Task<IEnumerable<ServiceCategoryDTO>> GetAllServiceCategoriesAsync();
        Task<ServiceCategoryDTO> GetServiceCategoryByIdAsync(int serviceCategoryId);
        Task<ServiceCategoryDTO> CreateServiceCategoryAsync(CreateServiceCategoryDTO serviceCategoryDTO);
        Task<bool> UpdateServiceCategoryAsync(int serviceCategoryId, UpdateServiceCategoryDTO serviceCategoryDTO);
        Task<bool> DeleteServiceCategoryAsync(int serviceCategoryId);
    }
}
