using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Take",
                table: "Prescription");

            migrationBuilder.AddColumn<string>(
                name: "MedicationForm",
                table: "PharmacyMedicationModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "PharmacyMedicationModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Schedule",
                table: "PharmacyMedicationModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicationForm",
                table: "PharmacyMedicationModel");

            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "PharmacyMedicationModel");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "PharmacyMedicationModel");

            migrationBuilder.AddColumn<string>(
                name: "Take",
                table: "Prescription",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
