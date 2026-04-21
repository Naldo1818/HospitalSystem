using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Migrations
{
    /// <inheritdoc />
    public partial class Stock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StockOrderedTable",
                table: "StockOrderedTable");

            migrationBuilder.RenameTable(
                name: "StockOrderedTable",
                newName: "OrderStockModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStockModel",
                table: "OrderStockModel",
                column: "OrderedStockID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStockModel",
                table: "OrderStockModel");

            migrationBuilder.RenameTable(
                name: "OrderStockModel",
                newName: "StockOrderedTable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockOrderedTable",
                table: "StockOrderedTable",
                column: "OrderedStockID");
        }
    }
}
