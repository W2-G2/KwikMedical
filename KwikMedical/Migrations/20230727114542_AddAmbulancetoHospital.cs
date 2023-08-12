using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KwikMedical.Migrations
{
    /// <inheritdoc />
    public partial class AddAmbulancetoHospital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "Ambulances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ambulances_HospitalId",
                table: "Ambulances",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ambulances_Hospitals_HospitalId",
                table: "Ambulances",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ambulances_Hospitals_HospitalId",
                table: "Ambulances");

            migrationBuilder.DropIndex(
                name: "IX_Ambulances_HospitalId",
                table: "Ambulances");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "Ambulances");
        }
    }
}
