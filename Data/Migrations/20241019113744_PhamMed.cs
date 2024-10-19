using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class PhamMed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DayHospitalPharmacyMedication",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "ActiveIngredientsDropDown",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "DosageForm",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "PharmMedDF",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "PharmMedSchedule",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.RenameTable(
                name: "DayHospitalPharmacyMedication",
                newName: "PharmacyMedicationModel");

            migrationBuilder.RenameColumn(
                name: "Schedule",
                table: "PharmacyMedicationModel",
                newName: "MedicationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmacyMedicationModel",
                table: "PharmacyMedicationModel",
                column: "PharmacyMedicationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmacyMedicationModel",
                table: "PharmacyMedicationModel");

            migrationBuilder.RenameTable(
                name: "PharmacyMedicationModel",
                newName: "DayHospitalPharmacyMedication");

            migrationBuilder.RenameColumn(
                name: "MedicationID",
                table: "DayHospitalPharmacyMedication",
                newName: "Schedule");

            migrationBuilder.AddColumn<string>(
                name: "ActiveIngredientsDropDown",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DosageForm",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PharmMedDF",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PharmMedSchedule",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DayHospitalPharmacyMedication",
                table: "DayHospitalPharmacyMedication",
                column: "PharmacyMedicationID");
        }
    }
}
