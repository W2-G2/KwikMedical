using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KwikMedical.Migrations
{
    /// <inheritdoc />
    public partial class heachaches : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_Ambulances_MedicalRecordId",
                table: "Ambulances");

            migrationBuilder.AddColumn<int>(
                name: "AmbulanceId",
                table: "MedicalRecords",
                type: "int",
                nullable: true);

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
                column: "AmbulanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ambulances_MedicalRecordId",
                table: "Ambulances",
                column: "MedicalRecordId",
                unique: true,
                filter: "[MedicalRecordId] IS NOT NULL");

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

            migrationBuilder.DropIndex(
                name: "IX_Ambulances_MedicalRecordId",
                table: "Ambulances");

            migrationBuilder.DropColumn(
                name: "AmbulanceId",
                table: "MedicalRecords");

            migrationBuilder.AlterColumn<int>(
                name: "AmbulanceId",
                table: "EmergencyCalls",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCalls_AmbulanceId",
                table: "EmergencyCalls",
                column: "AmbulanceId",
                unique: true,
                filter: "[AmbulanceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ambulances_MedicalRecordId",
                table: "Ambulances",
                column: "MedicalRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCalls_Ambulances_AmbulanceId",
                table: "EmergencyCalls",
                column: "AmbulanceId",
                principalTable: "Ambulances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
