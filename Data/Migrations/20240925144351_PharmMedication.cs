using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class PharmMedication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmMedModel",
                table: "PharmMedModel");

            migrationBuilder.DropColumn(
                name: "ActiveIngredientsAndStrength",
                table: "PharmMedModel");

            migrationBuilder.DropColumn(
                name: "DosageForm",
                table: "PharmMedModel");

            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "PharmMedModel");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "PharmMedModel");

            migrationBuilder.RenameTable(
                name: "PharmMedModel",
                newName: "PharmacyMedication");

            migrationBuilder.AddColumn<int>(
                name: "MedicationActiveingredientID",
                table: "PharmacyMedication",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MedicationID",
                table: "PharmacyMedication",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmacyMedication",
                table: "PharmacyMedication",
                column: "PharmacyMedicationlID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmacyMedication",
                table: "PharmacyMedication");

            migrationBuilder.DropColumn(
                name: "MedicationActiveingredientID",
                table: "PharmacyMedication");

            migrationBuilder.DropColumn(
                name: "MedicationID",
                table: "PharmacyMedication");

            migrationBuilder.RenameTable(
                name: "PharmacyMedication",
                newName: "PharmMedModel");

            migrationBuilder.AddColumn<string>(
                name: "ActiveIngredientsAndStrength",
                table: "PharmMedModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DosageForm",
                table: "PharmMedModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "PharmMedModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Schedule",
                table: "PharmMedModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmMedModel",
                table: "PharmMedModel",
                column: "PharmacyMedicationlID");
        }
    }
}
