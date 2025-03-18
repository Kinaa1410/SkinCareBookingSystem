using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkinCareBookingSystem.DTOs
{
    public class UserDetailsDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;
    }

    public class CreateUserDetailsDTO
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty; 
    }

    public class UpdateUserDetailsDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
