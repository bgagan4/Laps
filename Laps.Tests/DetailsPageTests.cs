using Laps.Web.Pages.Loans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Xunit;

namespace Laps.Tests
{
    public class DetailsPageTests : BasePageTests
    {
        [Fact]
        public async Task OnGetAsync_PopulatesPageModel_WithLoanApplication()
        {
            //Arrange
            var pageModel = new DetailsModel(AppDbContext);

            //Act
            var result = await pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsNotFoundResult_WithInvalidLoanApplication()
        {
            //Arrange
            var pageModel = new DetailsModel(AppDbContext);

            //Act
            var result = await pageModel.OnGetAsync(100);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
