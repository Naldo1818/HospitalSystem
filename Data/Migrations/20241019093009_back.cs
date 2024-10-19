using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class back : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DayHospitalPharmacyMedication_DayHospitalPharmacyMedication_PharmacyMedicationModelPharmacyMedicationID",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropIndex(
                name: "IX_DayHospitalPharmacyMedication_PharmacyMedicationModelPharmacyMedicationID",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "IngredientandStrength",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "PharmacyMedicationModelPharmacyMedicationID",
                table: "DayHospitalPharmacyMedication");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IngredientandStrength",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PharmacyMedicationModelPharmacyMedicationID",
                table: "DayHospitalPharmacyMedication",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DayHospitalPharmacyMedication_PharmacyMedicationModelPharmacyMedicationID",
                table: "DayHospitalPharmacyMedication",
                column: "PharmacyMedicationModelPharmacyMedicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_DayHospitalPharmacyMedication_DayHospitalPharmacyMedication_PharmacyMedicationModelPharmacyMedicationID",
                table: "DayHospitalPharmacyMedication",
                column: "PharmacyMedicationModelPharmacyMedicationID",
                principalTable: "DayHospitalPharmacyMedication",
                principalColumn: "PharmacyMedicationID");
        }
    }
}
