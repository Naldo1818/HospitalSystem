using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdmissionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdmissionStatus",
                table: "AdmittedPatients");

            migrationBuilder.AddColumn<int>(
                name: "AdmissionStatusId",
                table: "AdmittedPatients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AdmissionStatus",
                columns: table => new
                {
                    AdmissionStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionStatus", x => x.AdmissionStatusId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmittedPatients_AdmissionStatusId",
                table: "AdmittedPatients",
                column: "AdmissionStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmittedPatients_AdmissionStatus_AdmissionStatusId",
                table: "AdmittedPatients",
                column: "AdmissionStatusId",
                principalTable: "AdmissionStatus",
                principalColumn: "AdmissionStatusId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdmittedPatients_AdmissionStatus_AdmissionStatusId",
                table: "AdmittedPatients");

            migrationBuilder.DropTable(
                name: "AdmissionStatus");

            migrationBuilder.DropIndex(
                name: "IX_AdmittedPatients_AdmissionStatusId",
                table: "AdmittedPatients");

            migrationBuilder.DropColumn(
                name: "AdmissionStatusId",
                table: "AdmittedPatients");

            migrationBuilder.AddColumn<string>(
                name: "AdmissionStatus",
                table: "AdmittedPatients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
