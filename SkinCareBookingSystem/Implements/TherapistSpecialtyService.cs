using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Implements
{
    public class TherapistSpecialtyService : ITherapistSpecialtyService
    {
        private readonly BookingDbContext _context;

        public TherapistSpecialtyService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TherapistSpecialtyDTO>> GetAllTherapistSpecialtiesAsync()
        {
            return await _context.TherapistSpecialties
                .Include(ts => ts.Therapist)
                .Include(ts => ts.ServiceCategory)
                .Select(ts => new TherapistSpecialtyDTO
                {
                    Id = ts.Id,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.Therapist.UserName,
                    ServiceCategoryId = ts.ServiceCategoryId,
                    ServiceCategoryName = ts.ServiceCategory.Name
                }).ToListAsync();
        }

        public async Task<IEnumerable<TherapistSpecialtyDTO>> GetTherapistSpecialtiesByTherapistIdAsync(int therapistId)
        {
            return await _context.TherapistSpecialties
                .Where(ts => ts.TherapistId == therapistId)
                .Include(ts => ts.ServiceCategory)
                .Select(ts => new TherapistSpecialtyDTO
                {
                    Id = ts.Id,
                    ServiceCategoryId = ts.ServiceCategoryId,
                    ServiceCategoryName = ts.ServiceCategory.Name
                }).ToListAsync();
        }

        public async Task<IEnumerable<UserDTO>> GetTherapistsByServiceCategoryIdAsync(int serviceCategoryId)
        {
            return await _context.TherapistSpecialties
                .Where(ts => ts.ServiceCategoryId == serviceCategoryId)
                .Include(ts => ts.Therapist)
                .Select(ts => new UserDTO
                {
                    UserId = ts.Therapist.UserId,
                    UserName = ts.Therapist.UserName,
                    Email = ts.Therapist.Email,
                    Role = ts.Therapist.Role.RoleName,
                    Status = ts.Therapist.Status
                }).ToListAsync();
        }

        public async Task<TherapistSpecialtyDTO> CreateTherapistSpecialtyAsync(TherapistSpecialtyDTO specialtyDTO)
        {
            var therapistSpecialty = new TherapistSpecialty
            {
                TherapistId = specialtyDTO.TherapistId,
                ServiceCategoryId = specialtyDTO.ServiceCategoryId
            };

            _context.TherapistSpecialties.Add(therapistSpecialty);
            await _context.SaveChangesAsync();

            return new TherapistSpecialtyDTO
            {
                Id = therapistSpecialty.Id,
                TherapistId = therapistSpecialty.TherapistId,
                ServiceCategoryId = therapistSpecialty.ServiceCategoryId
            };
        }

        public async Task<bool> DeleteTherapistSpecialtyAsync(int id)
        {
            var specialty = await _context.TherapistSpecialties.FindAsync(id);
            if (specialty == null)
            {
                return false;
            }

            _context.TherapistSpecialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
