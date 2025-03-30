using SkinCareBookingSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class TherapistSpecialty
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Therapist")]
    public int TherapistId { get; set; }
    [ForeignKey("ServiceCategory")]
    public int ServiceCategoryId { get; set; }
    public User Therapist { get; set; } = null!;
    public ServiceCategory ServiceCategory { get; set; } = null!;
}
