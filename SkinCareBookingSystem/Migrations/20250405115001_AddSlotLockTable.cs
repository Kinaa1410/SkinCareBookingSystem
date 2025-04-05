using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinCareBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotLockTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TherapistTimeSlots_TimeSlotId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "TherapistTimeSlotId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TherapistTimeSlotLocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TherapistTimeSlotId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapistTimeSlotLocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TherapistTimeSlotLocks_TherapistTimeSlots_TherapistTimeSlotId",
                        column: x => x.TherapistTimeSlotId,
                        principalTable: "TherapistTimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TherapistTimeSlotId",
                table: "Bookings",
                column: "TherapistTimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_TherapistTimeSlotLocks_TherapistTimeSlotId",
                table: "TherapistTimeSlotLocks",
                column: "TherapistTimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TherapistTimeSlots_TherapistTimeSlotId",
                table: "Bookings",
                column: "TherapistTimeSlotId",
                principalTable: "TherapistTimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TherapistTimeSlots_TherapistTimeSlotId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "TherapistTimeSlotLocks");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TherapistTimeSlotId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TherapistTimeSlotId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TherapistTimeSlots_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId",
                principalTable: "TherapistTimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
