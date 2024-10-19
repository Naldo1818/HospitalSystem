using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class sa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveIngredientsDropDown",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IngredientandStrength",
                table: "DayHospitalPharmacyMedication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveIngredientsDropDown",
                table: "DayHospitalPharmacyMedication");

            migrationBuilder.DropColumn(
                name: "IngredientandStrength",
                table: "DayHospitalPharmacyMedication");
        }
    }
}
