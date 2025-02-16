using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [ForeignKey("ServiceCategory")]
        public int ServiceCategoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public float Price { get; set; } = 0.0f;

        [Required]
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        public string VideoURL { get; set; } = string.Empty;

        [Range(0, 5)]
        public float Rating { get; set; } = 0.0f;

        public bool Status { get; set; } = true;

        public bool Exist { get; set; } = true;

        // Navigation Property
        public ServiceCategory ServiceCategory { get; set; } = null!;
    }
}
