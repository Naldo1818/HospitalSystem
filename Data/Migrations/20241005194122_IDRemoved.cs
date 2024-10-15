using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class IDRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicationActiveingredientID",
                table: "PharmacyMedication");

         
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
     

            migrationBuilder.AddColumn<int>(
                name: "MedicationActiveingredientID",
                table: "PharmacyMedication",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
