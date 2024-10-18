using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class letsee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "PharmacyMedications",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PharmMedDF",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "PharmMedSchedule",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "PharmacyMedications",
                table: "DayHospitalPharmacyMedication");
        }
    }
}
