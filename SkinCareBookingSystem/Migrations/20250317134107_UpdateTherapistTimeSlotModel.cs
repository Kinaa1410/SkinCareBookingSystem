using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinCareBookingSystem.Migrations
{
    public partial class UpdateTherapistTimeSlotModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the new Status column
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TherapistTimeSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);  // Default to SlotStatus.Available (0)

            // Migrate data from IsAvailable (bool) to Status (SlotStatus enum)
            migrationBuilder.Sql("UPDATE TherapistTimeSlots SET Status = CASE WHEN IsAvailable = 1 THEN 0 ELSE 1 END");

            // Drop the old IsAvailable column after data migration
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "TherapistTimeSlots");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add the IsAvailable column back (for rollback)
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "TherapistTimeSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Drop the new Status column during rollback
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TherapistTimeSlots");
        }
    }
}
