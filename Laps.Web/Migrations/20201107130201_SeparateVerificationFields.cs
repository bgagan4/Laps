using Microsoft.EntityFrameworkCore.Migrations;

namespace Laps.Web.Migrations
{
    public partial class SeparateVerificationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreditScoreVerified",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "IsIncomeVerified",
                table: "LoanApplications");

            migrationBuilder.CreateTable(
                name: "ApplicationReviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(nullable: true),
                    ReviewerId = table.Column<string>(nullable: true),
                    IsIncomeVerified = table.Column<bool>(nullable: false),
                    IsCreditScoreVerified = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationReviews_LoanApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationReviews_ApplicationId",
                table: "ApplicationReviews",
                column: "ApplicationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationReviews");

            migrationBuilder.AddColumn<bool>(
                name: "IsCreditScoreVerified",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIncomeVerified",
                table: "LoanApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
