using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class AccountModalUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "Accounts",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Accounts",
                newName: "status");

            migrationBuilder.AlterColumn<int>(
                name: "ContactNumber",
                table: "Accounts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
