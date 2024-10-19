using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class hope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "PatientVitals");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "AdmittedPatients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "AdmittedPatients",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "AdmittedPatients");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "AdmittedPatients");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "PatientVitals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
