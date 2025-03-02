using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinCareBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTherapistTimeSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TherapistTimeSlots",
                columns: table => new
                {
                    TimeSlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapistTimeSlots", x => x.TimeSlotId);
                    table.ForeignKey(
                        name: "FK_TherapistTimeSlots_TherapistSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "TherapistSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TherapistTimeSlots_ScheduleId",
                table: "TherapistTimeSlots",
                column: "ScheduleId");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Bookings_BookingId",
                table: "BookingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TherapistSchedules_ScheduleId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TherapistSchedules_TherapistScheduleScheduleId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_StaffId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TherapistScheduleScheduleId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TherapistSchedules");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "TherapistSchedules");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TherapistSchedules");

            migrationBuilder.DropColumn(
                name: "TherapistScheduleScheduleId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "Bookings",
                newName: "TimeSlotId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ScheduleId",
                table: "Bookings",
                newName: "IX_Bookings_TimeSlotId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Bookings_BookingId",
                table: "BookingDetails",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TherapistTimeSlots_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId",
                principalTable: "TherapistTimeSlots",
                principalColumn: "TimeSlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_StaffId",
                table: "Bookings",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryId",
                table: "Services",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "ServiceCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Bookings_BookingId",
                table: "BookingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TherapistTimeSlots_TimeSlotId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_StaffId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "TherapistTimeSlots");

            migrationBuilder.RenameColumn(
                name: "TimeSlotId",
                table: "Bookings",
                newName: "ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings",
                newName: "IX_Bookings_ScheduleId");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "TherapistSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "TherapistSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "TherapistSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "TherapistScheduleScheduleId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TherapistScheduleScheduleId",
                table: "Bookings",
                column: "TherapistScheduleScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Bookings_BookingId",
                table: "BookingDetails",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TherapistSchedules_ScheduleId",
                table: "Bookings",
                column: "ScheduleId",
                principalTable: "TherapistSchedules",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TherapistSchedules_TherapistScheduleScheduleId",
                table: "Bookings",
                column: "TherapistScheduleScheduleId",
                principalTable: "TherapistSchedules",
                principalColumn: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_StaffId",
                table: "Bookings",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryId",
                table: "Services",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "ServiceCategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
