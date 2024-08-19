using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class MedicationActiveIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveIngredientStrength",
                table: "Activeingredient");

            migrationBuilder.DropColumn(
                name: "MedicationID",
                table: "Activeingredient");

            migrationBuilder.CreateTable(
                name: "MedicationActiveIngredient",
                columns: table => new
                {
                    MedicationActiveingredientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationID = table.Column<int>(type: "int", nullable: false),
                    ActiveingredientID = table.Column<int>(type: "int", nullable: false),
                    ActiveIngredientStrength = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationActiveIngredient", x => x.MedicationActiveingredientID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicationActiveIngredient");

            migrationBuilder.AddColumn<int>(
                name: "ActiveIngredientStrength",
                table: "Activeingredient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MedicationID",
                table: "Activeingredient",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
