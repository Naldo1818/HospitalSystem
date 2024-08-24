using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class Pharmacymedicationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "PatientVitals",
                newName: "Weight");

            migrationBuilder.AddColumn<int>(
                name: "BloodGlucoseLevel",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BloodOxygen",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiastolicBloodPressure",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HeartRate",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Respiration",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SystolicBloodPressure",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Temperature",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "time",
                table: "PatientVitals",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateTable(
                name: "PharmMeds",
                columns: table => new
                {
                    PharmacyMedicationlID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DosageForm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Schedule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockonHand = table.Column<int>(type: "int", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    ActiveIngredientsAndStrength = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmMeds", x => x.PharmacyMedicationlID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PharmMeds");

            migrationBuilder.DropColumn(
                name: "BloodGlucoseLevel",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "BloodOxygen",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "DiastolicBloodPressure",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "Respiration",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "SystolicBloodPressure",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "time",
                table: "PatientVitals");

            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "PatientVitals",
                newName: "Value");
        }
    }
}
