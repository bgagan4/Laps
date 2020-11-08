using Microsoft.EntityFrameworkCore.Migrations;

namespace Laps.Web.Migrations
{
    public partial class UpdateApplicationClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinCode",
                table: "LoanApplications");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "LoanApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "LoanApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Zip",
                table: "LoanApplications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "State",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "Zip",
                table: "LoanApplications");

            migrationBuilder.AddColumn<string>(
                name: "PinCode",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
