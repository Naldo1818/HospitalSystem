using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEMO.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameBookingIDToAdmittedID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookingID",
                table: "Prescription",
                newName: "AdmittedPatientID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BedId",
                table: "AdmittedPatients");

            migrationBuilder.RenameColumn(
                name: "AdmittedPatientID",
                table: "Prescription",
                newName: "BookingID");
        }
    }
}
