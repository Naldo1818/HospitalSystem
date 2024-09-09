using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class Zaid2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdmittedPatientID",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "VitalsID",
                table: "PatientVitals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdmittedPatientID",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VitalsID",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
