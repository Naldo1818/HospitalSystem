using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Migrations
{
    /// <inheritdoc />
    public partial class database2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "AdmittedPatients");

            migrationBuilder.DropColumn(
                name: "PatientID",
                table: "AdmittedPatients");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "AdmittedPatients");

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "ContactInfo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ContactNumber",
                table: "ContactInfo",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "AdmittedPatients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientID",
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
    }
}
