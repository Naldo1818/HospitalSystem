using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IngredientandStrength",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "IngredientsplusStrength",
                table: "DayHospitalPharmacyMedication");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IngredientandStrength",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IngredientsplusStrength",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
