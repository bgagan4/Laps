using Laps.Web.Authorization;
using Laps.Web.Pages.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace Laps.Tests
{
    public class IndexPageTests : BasePageTests
    {
        [Fact]
        public async Task OnGetAsync_PopulatesPageModel_ForOwner()
        {
            //Arrange
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var store = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            //Create test user
            var displayName = "User name";
            var identity = new GenericIdentity(displayName);
            var principleMock = new Mock<ClaimsPrincipal>(identity);
            principleMock.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);

            userManager.Setup(x => x.GetUserId(principleMock.Object)).Returns("test.user");
            // use default context with user
            var httpContext = new DefaultHttpContext()
            {
                User = principleMock.Object
            };
            //need these as well for the page context
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            // need page context for the page model
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            var pageModel = new IndexModel(AppDbContext, authorizationService.Object, userManager.Object) { PageContext = pageContext };

            //Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.LoanApplications);
            Assert.Equal(1, pageModel.LoanApplications.Count);
        }

        [Fact]
        public async Task OnGetAsync_PopulatesPageModel_ForProcessor()
        {
            //Arrange
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var store = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            //Create test user
            var displayName = "User name";
            var identity = new GenericIdentity(displayName);
            var principleMock = new Mock<ClaimsPrincipal>(identity);
            principleMock.Setup(x => x.IsInRole(Constants.LoanProcessorsRole)).Returns(true);

            userManager.Setup(x => x.GetUserId(principleMock.Object)).Returns("test.user");
            // use default context with user
            var httpContext = new DefaultHttpContext()
            {
                User = principleMock.Object
            };
            //need these as well for the page context
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            // need page context for the page model
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            var pageModel = new IndexModel(AppDbContext, authorizationService.Object, userManager.Object) { PageContext = pageContext };

            //Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.LoanApplications);
            Assert.Equal(2, pageModel.LoanApplications.Count);
        }

        [Fact]
        public async Task OnGetAsync_PopulatesPageModel_ForManager()
        {
            //Arrange
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var store = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            //Create test user
            var displayName = "User name";
            var identity = new GenericIdentity(displayName);
            var principleMock = new Mock<ClaimsPrincipal>(identity);
            principleMock.Setup(x => x.IsInRole(Constants.LoanManagersRole)).Returns(true);

            userManager.Setup(x => x.GetUserId(principleMock.Object)).Returns("test.user");
            // use default context with user
            var httpContext = new DefaultHttpContext()
            {
                User = principleMock.Object
            };
            //need these as well for the page context
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            // need page context for the page model
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            var pageModel = new IndexModel(AppDbContext, authorizationService.Object, userManager.Object) { PageContext = pageContext };

            //Act
            await pageModel.OnGetAsync(true);

            // Assert
            Assert.NotNull(pageModel.LoanApplications);
            Assert.Equal(3, pageModel.LoanApplications.Count);
        }

    }
}
