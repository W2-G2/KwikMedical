using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KwikMedical.Migrations
{
    /// <inheritdoc />
    public partial class DataClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyCalls_Ambulances_AmbulanceId",
                table: "EmergencyCalls");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCalls_AmbulanceId",
                table: "EmergencyCalls");

            migrationBuilder.AlterColumn<int>(
                name: "AmbulanceId",
                table: "EmergencyCalls",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CurrentEmergencyCallId",
                table: "Ambulances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCalls_AmbulanceId",
                table: "EmergencyCalls",
                column: "AmbulanceId",
                unique: true,
                filter: "[AmbulanceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCalls_Ambulances_AmbulanceId",
                table: "EmergencyCalls",
                column: "AmbulanceId",
                principalTable: "Ambulances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
                name: "CurrentEmergencyCallId",
                table: "Ambulances");

            migrationBuilder.AlterColumn<int>(
                name: "AmbulanceId",
                table: "EmergencyCalls",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
