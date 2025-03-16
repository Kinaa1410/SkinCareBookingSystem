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

        public async Task<TherapistSpecialtyDTO> UpdateTherapistSpecialtyAsync(int therapistId, int serviceCategoryId)
        {
            // Check if the therapistId exists in the database and if the user is a therapist (RoleId == 2)
            var therapist = await _context.Users.FirstOrDefaultAsync(u => u.UserId == therapistId && u.RoleId == 2);  // RoleId == 2 indicates "Therapist"

            // If the therapist doesn't exist or is not a therapist, throw an error
            if (therapist == null)
            {
                throw new KeyNotFoundException("Therapist with the given ID not found or user is not a therapist.");
            }

            // Check if the serviceCategoryId exists in the ServiceCategories table
            var serviceCategory = await _context.ServiceCategories
                .FirstOrDefaultAsync(sc => sc.ServiceCategoryId == serviceCategoryId);

            // If the service category doesn't exist, throw an error
            if (serviceCategory == null)
            {
                throw new KeyNotFoundException("Service category with the given ID not found.");
            }

            // Find the therapist specialty by therapistId
            var therapistSpecialty = await _context.TherapistSpecialties
                .FirstOrDefaultAsync(ts => ts.TherapistId == therapistId);

            // If the therapist specialty doesn't exist, throw an error
            if (therapistSpecialty == null)
            {
                throw new KeyNotFoundException("Therapist specialty not found.");
            }

            // Update the service category ID
            therapistSpecialty.ServiceCategoryId = serviceCategoryId;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated specialty
            return new TherapistSpecialtyDTO
            {
                Id = therapistSpecialty.Id,
                TherapistId = therapistSpecialty.TherapistId,
                ServiceCategoryId = therapistSpecialty.ServiceCategoryId
            };
        }


    }
}
