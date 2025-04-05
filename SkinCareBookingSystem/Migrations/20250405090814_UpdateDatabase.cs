using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinCareBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_StaffId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_QaAnswers_Users_UserId",
                table: "QaAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_Qas_ServiceCategories_ServiceCategoryId",
                table: "Qas");

            migrationBuilder.DropTable(
                name: "ServiceRecommendations");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_StaffId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Answer",
                table: "QaAnswers");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "QaOptionId",
                table: "QaAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TherapistId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QaOptions",
                columns: table => new
                {
                    QaOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QaOptions", x => x.QaOptionId);
                    table.ForeignKey(
                        name: "FK_QaOptions_Qas_QaId",
                        column: x => x.QaId,
                        principalTable: "Qas",
                        principalColumn: "QaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QaOptionServices",
                columns: table => new
                {
                    QaOptionServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QaOptionId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QaOptionServices", x => x.QaOptionServiceId);
                    table.ForeignKey(
                        name: "FK_QaOptionServices_QaOptions_QaOptionId",
                        column: x => x.QaOptionId,
                        principalTable: "QaOptions",
                        principalColumn: "QaOptionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QaOptionServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QaAnswers_QaOptionId",
                table: "QaAnswers",
                column: "QaOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ServiceId",
                table: "Bookings",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TherapistId",
                table: "Bookings",
                column: "TherapistId");

            migrationBuilder.CreateIndex(
                name: "IX_QaOptions_QaId",
                table: "QaOptions",
                column: "QaId");

            migrationBuilder.CreateIndex(
                name: "IX_QaOptionServices_QaOptionId",
                table: "QaOptionServices",
                column: "QaOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QaOptionServices_ServiceId",
                table: "QaOptionServices",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                table: "Bookings",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_TherapistId",
                table: "Bookings",
                column: "TherapistId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QaAnswers_QaOptions_QaOptionId",
                table: "QaAnswers",
                column: "QaOptionId",
                principalTable: "QaOptions",
                principalColumn: "QaOptionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QaAnswers_Users_UserId",
                table: "QaAnswers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Qas_ServiceCategories_ServiceCategoryId",
                table: "Qas",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "ServiceCategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_TherapistId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_QaAnswers_QaOptions_QaOptionId",
                table: "QaAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QaAnswers_Users_UserId",
                table: "QaAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_Qas_ServiceCategories_ServiceCategoryId",
                table: "Qas");

            migrationBuilder.DropTable(
                name: "QaOptionServices");

            migrationBuilder.DropTable(
                name: "QaOptions");

            migrationBuilder.DropIndex(
                name: "IX_QaAnswers_QaOptionId",
                table: "QaAnswers");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ServiceId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TherapistId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "QaOptionId",
                table: "QaAnswers");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TherapistId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "QaAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceRecommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QaId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    AnswerOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRecommendations_Qas_QaId",
                        column: x => x.QaId,
                        principalTable: "Qas",
                        principalColumn: "QaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRecommendations_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StaffId",
                table: "Bookings",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecommendations_QaId",
                table: "ServiceRecommendations",
                column: "QaId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecommendations_ServiceId",
                table: "ServiceRecommendations",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_StaffId",
                table: "Bookings",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_QaAnswers_Users_UserId",
                table: "QaAnswers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Qas_ServiceCategories_ServiceCategoryId",
                table: "Qas",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "ServiceCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
