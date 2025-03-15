using FluentValidation;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace SkinCareBookingSystem.Validators
{
    public class CreateTherapistTimeSlotDTOValidator : AbstractValidator<CreateTherapistTimeSlotDTO>
    {
        private readonly BookingDbContext _context;

        public CreateTherapistTimeSlotDTOValidator(BookingDbContext context)
        {
            _context = context;

            RuleFor(x => x.TimeSlotId)
                .MustAsync(async (id, cancellationToken) => await _context.TimeSlots.AnyAsync(t => t.TimeSlotId == id))
                .WithMessage("The specified TimeSlotId does not exist.");
        }
    }

    public class UpdateTherapistTimeSlotDTOValidator : AbstractValidator<UpdateTherapistTimeSlotDTO>
    {
        private readonly BookingDbContext _context;

        public UpdateTherapistTimeSlotDTOValidator(BookingDbContext context)
        {
            _context = context;

            RuleFor(x => x.TimeSlotId)
                .MustAsync(async (id, cancellationToken) => await _context.TimeSlots.AnyAsync(t => t.TimeSlotId == id))
                .WithMessage("The specified TimeSlotId does not exist.");
        }
    }
}
