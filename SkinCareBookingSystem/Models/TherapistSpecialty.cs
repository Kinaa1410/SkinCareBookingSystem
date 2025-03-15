using SkinCareBookingSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class TherapistSpecialty
{
    [Key]
    public int Id { get; set; }

    // Foreign Key for Therapist (User)
    [ForeignKey("Therapist")]
    public int TherapistId { get; set; }

    // Foreign Key for ServiceCategory (Specialty)
    [ForeignKey("ServiceCategory")]
    public int ServiceCategoryId { get; set; }

    // Navigation property to User (Therapist)
    public User Therapist { get; set; } = null!;

    // Navigation property to ServiceCategory (Specialty)
    public ServiceCategory ServiceCategory { get; set; } = null!;
}
