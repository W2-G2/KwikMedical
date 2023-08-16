using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KwikMedical.Migrations
{
    /// <inheritdoc />
    public partial class ohboy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "EmergencyCalls",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "EmergencyCalls");
        }
    }
}
