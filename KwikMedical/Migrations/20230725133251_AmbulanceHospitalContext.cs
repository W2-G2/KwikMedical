using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KwikMedical.Migrations
{
    /// <inheritdoc />
    public partial class AmbulanceHospitalContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmbulanceId",
                table: "EmergencyCalls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCity",
                table: "Ambulances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCalls_AmbulanceId",
                table: "EmergencyCalls",
                column: "AmbulanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCalls_Ambulances_AmbulanceId",
                table: "EmergencyCalls",
                column: "AmbulanceId",
                principalTable: "Ambulances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyCalls_Ambulances_AmbulanceId",
                table: "EmergencyCalls");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCalls_AmbulanceId",
                table: "EmergencyCalls");

            migrationBuilder.DropColumn(
                name: "AmbulanceId",
                table: "EmergencyCalls");

            migrationBuilder.DropColumn(
                name: "CurrentCity",
                table: "Ambulances");
        }
    }
}
