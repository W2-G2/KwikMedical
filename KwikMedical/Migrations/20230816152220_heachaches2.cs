using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KwikMedical.Migrations
{
    /// <inheritdoc />
    public partial class heachaches2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedAmbulanceId",
                table: "Ambulances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ambulances_AssignedAmbulanceId",
                table: "Ambulances",
                column: "AssignedAmbulanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ambulances_Ambulances_AssignedAmbulanceId",
                table: "Ambulances",
                column: "AssignedAmbulanceId",
                principalTable: "Ambulances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ambulances_Ambulances_AssignedAmbulanceId",
                table: "Ambulances");

            migrationBuilder.DropIndex(
                name: "IX_Ambulances_AssignedAmbulanceId",
                table: "Ambulances");

            migrationBuilder.DropColumn(
                name: "AssignedAmbulanceId",
                table: "Ambulances");
        }
    }
}
