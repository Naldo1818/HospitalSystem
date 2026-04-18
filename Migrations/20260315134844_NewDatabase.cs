using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentMedication");

            migrationBuilder.DropColumn(
                name: "MedicationForm",
                table: "PharmacyMedicationModel");

            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "PharmacyMedicationModel");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "PharmacyMedicationModel");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "PatientInfo");

            migrationBuilder.DropColumn(
                name: "AdmissionStatusID",
                table: "AdmittedPatients");

            migrationBuilder.RenameColumn(
                name: "MedicationID",
                table: "patientMedication",
                newName: "CMedicationID");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BookSurgery",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ChronicMedication",
                columns: table => new
                {
                    CMedicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CMedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CMedicationForm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Schedule = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChronicMedication", x => x.CMedicationID);
                });

            migrationBuilder.CreateTable(
                name: "ChronicMedicationActiveIngredient",
                columns: table => new
                {
                    CMedicationActiveingredientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CMedicationID = table.Column<int>(type: "int", nullable: false),
                    ActiveingredientID = table.Column<int>(type: "int", nullable: false),
                    ActiveIngredientStrength = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChronicMedicationActiveIngredient", x => x.CMedicationActiveingredientID);
                });

            migrationBuilder.CreateTable(
                name: "ContactInfo",
                columns: table => new
                {
                    ContactInfoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    ContactNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactInfo", x => x.ContactInfoID);
                });

            migrationBuilder.CreateTable(
                name: "Discharged",
                columns: table => new
                {
                    DischargeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingID = table.Column<int>(type: "int", nullable: false),
                    Notw = table.Column<bool>(type: "bit", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discharged", x => x.DischargeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChronicMedication");

            migrationBuilder.DropTable(
                name: "ChronicMedicationActiveIngredient");

            migrationBuilder.DropTable(
                name: "ContactInfo");

            migrationBuilder.DropTable(
                name: "Discharged");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookSurgery");

            migrationBuilder.RenameColumn(
                name: "CMedicationID",
                table: "patientMedication",
                newName: "MedicationID");

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

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "PatientInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AdmissionStatusID",
                table: "AdmittedPatients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CurrentMedication",
                columns: table => new
                {
                    CurrentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentMedication", x => x.CurrentId);
                });
        }
    }
}
