using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateYourDateColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IDNumber",
                table: "PatientInfo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IDNumber",
                table: "PatientInfo",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
