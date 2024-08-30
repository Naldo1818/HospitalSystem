using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class SamZaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "PatientAllergy",
            //    columns: table => new
            //    {
            //        patientAllergyID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        PatientID = table.Column<int>(type: "int", nullable: false),
            //        ActiveingredientID = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PatientAllergy", x => x.patientAllergyID);
            //    });

            migrationBuilder.CreateTable(
                name: "RejectScriptModel",
                columns: table => new
                {
                    RejectionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrescriptionID = table.Column<int>(type: "int", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectScriptModel", x => x.RejectionID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientAllergy");

            migrationBuilder.DropTable(
                name: "RejectScriptModel");
        }
    }
}
