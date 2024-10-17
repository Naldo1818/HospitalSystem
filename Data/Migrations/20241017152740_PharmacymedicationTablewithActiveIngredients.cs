using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class PharmacymedicationTablewithActiveIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DosageForm",
                table: "PharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "PharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Schedule",
                table: "PharmacyMedication",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountID",
                table: "AdmittedPatients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DayHospitalPharmacyMedication",
                columns: table => new
                {
                    PharmacyMedicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DosageForm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Schedule = table.Column<int>(type: "int", nullable: false),
                    StockonHand = table.Column<int>(type: "int", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    IngredientandStrength = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IngredientsplusStrength = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayHospitalPharmacyMedication", x => x.PharmacyMedicationID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "DosageForm",
                table: "PharmacyMedication");

            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "PharmacyMedication");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "PharmacyMedication");

            migrationBuilder.DropColumn(
                name: "AccountID",
                table: "AdmittedPatients");
        }
    }
}
