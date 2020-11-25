using Laps.Web.Data.Models;
using Laps.Web.Pages.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Laps.Tests
{
    public class WithdrawPageTests : BasePageTests
    {
        [Fact]
        public async Task OnGetAsync_PopulatesPageModel_WithLoanApplication()
        {
            //Arrange
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var store = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var pageModel = new WithdrawModel(AppDbContext, authorizationService.Object, userManager.Object);

            //Act
            var result = await pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsNotFoundResult_WithInvalidLoanApplication()
        {
            //Arrange
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var store = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var pageModel = new WithdrawModel(AppDbContext, authorizationService.Object, userManager.Object);

            //Act
            var result = await pageModel.OnGetAsync(100);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsForbidResult_WithUnAuthorizedUser()
        {
            //Arrange
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var store = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var pageModel = new WithdrawModel(AppDbContext, authorizationService.Object, userManager.Object);

            //Act
            var result = await pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task OnPostWithdrawAsync_ReturnsARedirectToPageResult_WhenModelStateIsValid()
        {
            // Arrange
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var store = new Mock<IUserStore<IdentityUser>>();
            var mgr = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var loanId = 1;
            var loanApplication = await AppDbContext.LoanApplications.AsNoTracking().FirstOrDefaultAsync(m => m.Id == loanId);
            var pageModel = new WithdrawModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostAsync(loanId);

            var approvalApplication = await AppDbContext.LoanApplications.FirstOrDefaultAsync(m => m.Id == loanId);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(LoanStatus.Withdrawn, approvalApplication.Status);
        }

        [Fact]
        public async Task OnPostWithdrawAsync_ReturnsNotFoundResult_WhenModelStateIsValid()
        {
            // Arrange
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var store = new Mock<IUserStore<IdentityUser>>();
            var mgr = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var loanId = 100;
            var loanApplication = await AppDbContext.LoanApplications.AsNoTracking().FirstOrDefaultAsync(m => m.Id == loanId);
            var pageModel = new WithdrawModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostAsync(loanId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
