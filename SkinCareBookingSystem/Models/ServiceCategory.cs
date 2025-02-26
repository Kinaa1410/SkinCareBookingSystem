using System.ComponentModel.DataAnnotations;

public class ServiceCategory
{
    [Key]
    public int ServiceCategoryId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public bool Status { get; set; }
    public bool Exist { get; set; }

    public ICollection<Service> Services { get; set; } = new List<Service>();
}
