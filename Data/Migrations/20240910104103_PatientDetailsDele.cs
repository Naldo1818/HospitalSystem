using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class PatientDetailsDele : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientDetailsID",
                table: "AdmittedPatients",
                newName: "AddressID");

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PatientVitals",
                columns: table => new
                {
                    PatientVitalsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmittedPatientID = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    SystolicBloodPressure = table.Column<int>(type: "int", nullable: false),
                    DiastolicBloodPressure = table.Column<int>(type: "int", nullable: false),
                    HeartRate = table.Column<int>(type: "int", nullable: false),
                    BloodOxygen = table.Column<int>(type: "int", nullable: false),
                    Respiration = table.Column<int>(type: "int", nullable: false),
                    BloodGlucoseLevel = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<int>(type: "int", nullable: false),
                    time = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVitals", x => x.PatientVitalsID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "Address");

            migrationBuilder.RenameColumn(
                name: "AddressID",
                table: "AdmittedPatients",
                newName: "PatientDetailsID");
        }
    }
}
