using Microsoft.EntityFrameworkCore.Migrations;

namespace SkinCareBookingSystem.Migrations
{
    public partial class ChangeBookingStatusToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Map existing boolean values to enum values
            migrationBuilder.Sql("UPDATE Bookings SET Status = 1 WHERE Status = 1"); // true -> Booked (1)
            migrationBuilder.Sql("UPDATE Bookings SET Status = 4 WHERE Status = 0"); // false -> Failed (4)

            // Step 2: Change the column type from bit to int
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Map enum values back to boolean (simplified rollback)
            migrationBuilder.Sql("UPDATE Bookings SET Status = 1 WHERE Status = 1"); // Booked -> true
            migrationBuilder.Sql("UPDATE Bookings SET Status = 0 WHERE Status IN (0, 2, 3, 4)"); // Pending, Completed, Canceled, Failed -> false

            // Step 2: Change the column type back to bit
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Bookings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}