using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinCareBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Bookings_BookingId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_BookingId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "CommentService",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "RatingTherapist",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "RatingService",
                table: "Feedbacks",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "CommentTherapist",
                table: "Feedbacks",
                newName: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0); // Adjust default value if needed

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ServiceId",
                table: "Feedbacks",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Services_ServiceId",
                table: "Feedbacks",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Services_ServiceId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_ServiceId",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Feedbacks",
                newName: "RatingTherapist");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Feedbacks",
                newName: "RatingService");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Feedbacks",
                newName: "CommentTherapist");

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CommentService",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Feedbacks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Feedbacks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_BookingId",
                table: "Feedbacks",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Bookings_BookingId",
                table: "Feedbacks",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
