using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class PhamMed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DosageForm",
                table: "PharmacyMedication");

            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "PharmacyMedication");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "PharmacyMedication");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DosageForm",
                table: "PharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "PharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Schedule",
                table: "PharmacyMedication",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
