using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Migrations
{
    /// <inheritdoc />
    public partial class Date : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "time",
                table: "PatientVitals",
                newName: "Time");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "PatientVitals",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "PatientVitals");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "PatientVitals",
                newName: "time");
        }
    }
}
