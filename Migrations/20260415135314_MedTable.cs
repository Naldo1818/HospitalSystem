using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Migrations
{
    /// <inheritdoc />
    public partial class MedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PharmacyMedication");

            migrationBuilder.DropTable(
                name: "PharmacyMedicationModel");

            migrationBuilder.DropTable(
                name: "PharmacyStock");

            migrationBuilder.DropTable(
                name: "ReceivedStock");

            migrationBuilder.DropColumn(
                name: "DosageForm",
                table: "StockOrderedTable");

            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "StockOrderedTable");

            migrationBuilder.DropColumn(
                name: "PharmacistName",
                table: "StockOrderedTable");

            migrationBuilder.DropColumn(
                name: "PharmacistSurname",
                table: "StockOrderedTable");

            migrationBuilder.DropColumn(
                name: "PharmacyMedicationID",
                table: "StockOrderedTable");

            migrationBuilder.RenameColumn(
                name: "StockonHand",
                table: "StockOrderedTable",
                newName: "MedicationID");

            migrationBuilder.RenameColumn(
                name: "Schedule",
                table: "StockOrderedTable",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "ReorderLevel",
                table: "StockOrderedTable",
                newName: "AccountID");

            migrationBuilder.RenameColumn(
                name: "Notw",
                table: "Discharged",
                newName: "Note");

            migrationBuilder.AddColumn<int>(
                name: "ReorderLevel",
                table: "Medication",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockonHand",
                table: "Medication",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Discharged",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReorderLevel",
                table: "Medication");

            migrationBuilder.DropColumn(
                name: "StockonHand",
                table: "Medication");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Discharged");

            migrationBuilder.RenameColumn(
                name: "MedicationID",
                table: "StockOrderedTable",
                newName: "StockonHand");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "StockOrderedTable",
                newName: "Schedule");

            migrationBuilder.RenameColumn(
                name: "AccountID",
                table: "StockOrderedTable",
                newName: "ReorderLevel");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Discharged",
                newName: "Notw");

            migrationBuilder.AddColumn<string>(
                name: "DosageForm",
                table: "StockOrderedTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "StockOrderedTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PharmacistName",
                table: "StockOrderedTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PharmacistSurname",
                table: "StockOrderedTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PharmacyMedicationID",
                table: "StockOrderedTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PharmacyMedication",
                columns: table => new
                {
                    PharmacyMedicationlID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationID = table.Column<int>(type: "int", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    StockonHand = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacyMedication", x => x.PharmacyMedicationlID);
                });

            migrationBuilder.CreateTable(
                name: "PharmacyMedicationModel",
                columns: table => new
                {
                    PharmacyMedicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationID = table.Column<int>(type: "int", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    StockonHand = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacyMedicationModel", x => x.PharmacyMedicationID);
                });

            migrationBuilder.CreateTable(
                name: "PharmacyStock",
                columns: table => new
                {
                    MedicationReorderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationForm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    Schedule = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockonHand = table.Column<int>(type: "int", nullable: false),
                    qtyOrdered = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacyStock", x => x.MedicationReorderID);
                });

            migrationBuilder.CreateTable(
                name: "ReceivedStock",
                columns: table => new
                {
                    ReceivedStockID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationForm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicationID = table.Column<int>(type: "int", nullable: false),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Schedule = table.Column<int>(type: "int", nullable: false),
                    qtyReceived = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivedStock", x => x.ReceivedStockID);
                });
        }
    }
}
