using Microsoft.EntityFrameworkCore.Migrations;

namespace Laps.Web.Migrations
{
    public partial class AddVerificationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCreditScoreVerified",
                table: "LoanApplications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIncomeVerified",
                table: "LoanApplications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreditScoreVerified",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsIncomeVerified",
                table: "LoanApplications");
        }
    }
}
