using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class Zaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ActiveingredientID",
                table: "PatientAllergy",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "AdmittedPatients",
                columns: table => new
                {
                    AdmittedPatientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    BookingID = table.Column<int>(type: "int", nullable: false),
                    WardID = table.Column<int>(type: "int", nullable: false),
                    PatientVitalsID = table.Column<int>(type: "int", nullable: false),
                    PatientConditionsID = table.Column<int>(type: "int", nullable: false),
                    PatientMedicationID = table.Column<int>(type: "int", nullable: false),
                    PatientAllergyID = table.Column<int>(type: "int", nullable: false),
                    PatientDetailsID = table.Column<int>(type: "int", nullable: false),
                    AdmissionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmittedPatients", x => x.AdmittedPatientID);
                });

            migrationBuilder.CreateTable(
                name: "Condition",
                columns: table => new
                {
                    ConditionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConditionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condition", x => x.ConditionID);
                });

            //migrationBuilder.CreateTable(
            //    name: "CurrentMedication",
            //    columns: table => new
            //    {
            //        CurrentId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CurrentMedication", x => x.CurrentId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PatientConditions",
            //    columns: table => new
            //    {
            //        PatientConditionsID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        PatientID = table.Column<int>(type: "int", nullable: false),
            //        ConditionsID = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PatientConditions", x => x.PatientConditionsID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PatientDetails",
            //    columns: table => new
            //    {
            //        PatientDetailsID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        AdmittedPatientID = table.Column<int>(type: "int", nullable: false),
            //        AddressID = table.Column<int>(type: "int", nullable: false),
            //        CityID = table.Column<int>(type: "int", nullable: false),
            //        ProvinceID = table.Column<int>(type: "int", nullable: false),
            //        SuburbID = table.Column<int>(type: "int", nullable: false),
            //        StreetName = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PatientDetails", x => x.PatientDetailsID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "patientMedication",
            //    columns: table => new
            //    {
            //        PatientMedicationID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        PatientID = table.Column<int>(type: "int", nullable: false),
            //        CurrentID = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_patientMedication", x => x.PatientMedicationID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PatientVitals",
            //    columns: table => new
            //    {
            //        PatientVitalsID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        VitalsID = table.Column<int>(type: "int", nullable: false),
            //        AdmittedPatientID = table.Column<int>(type: "int", nullable: false),
            //        Height = table.Column<int>(type: "int", nullable: false),
            //        Weight = table.Column<int>(type: "int", nullable: false),
            //        SystolicBloodPressure = table.Column<int>(type: "int", nullable: false),
            //        DiastolicBloodPressure = table.Column<int>(type: "int", nullable: false),
            //        HeartRate = table.Column<int>(type: "int", nullable: false),
            //        BloodOxygen = table.Column<int>(type: "int", nullable: false),
            //        Respiration = table.Column<int>(type: "int", nullable: false),
            //        BloodGlucoseLevel = table.Column<int>(type: "int", nullable: false),
            //        Temperature = table.Column<int>(type: "int", nullable: false),
            //        time = table.Column<TimeOnly>(type: "time", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PatientVitals", x => x.PatientVitalsID);
            //    });

            migrationBuilder.CreateTable(
                name: "PharmMedModel",
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
                    table.PrimaryKey("PK_PharmMedModel", x => x.PharmacyMedicationlID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmittedPatients");

            migrationBuilder.DropTable(
                name: "Condition");

            migrationBuilder.DropTable(
                name: "CurrentMedication");

            migrationBuilder.DropTable(
                name: "PatientConditions");

            migrationBuilder.DropTable(
                name: "PatientDetails");

            migrationBuilder.DropTable(
                name: "patientMedication");

            migrationBuilder.DropTable(
                name: "PatientVitals");

            migrationBuilder.DropTable(
                name: "PharmMedModel");

            migrationBuilder.AlterColumn<string>(
                name: "ActiveingredientID",
                table: "PatientAllergy",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
