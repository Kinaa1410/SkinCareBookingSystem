namespace SkinCareBookingSystem.DTOs
{
    public class RoleDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class CreateRoleDTO
    {
        public string RoleName { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class UpdateRoleDTO
    {
        public string RoleName { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
