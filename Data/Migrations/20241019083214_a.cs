using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DayHospitalPharmacyMedication_DayHospitalPharmacyMedication_PharmacyMedicationModelPharmacyMedicationID",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropIndex(
                name: "IX_DayHospitalPharmacyMedication_PharmacyMedicationModelPharmacyMedicationID",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "PharmacyMedicationModelPharmacyMedicationID",
                table: "DayHospitalPharmacyMedication");
        }
    }
}
