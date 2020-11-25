using Laps.Web.Authorization;
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
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace Laps.Tests
{
    public class ReviewPageTests : BasePageTests
    {
        [Fact]
        public async Task OnGetAsync_PopulatesPageModel_WithValidLoanApplication()
        {
            //Arrange
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var store = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, userManager.Object);

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
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, userManager.Object);

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
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, userManager.Object);

            //Act
            var result = await pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task OnPostApprovalAsync_ReturnsARedirectToPageResult_WhenModelStateIsValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostApprovalAsync();

            var approvalApplication = await AppDbContext.LoanApplications.FirstOrDefaultAsync(m => m.Id == loanId);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(LoanStatus.AwaitingManagersApproval, approvalApplication.Status);
        }

        [Fact]
        public async Task OnPostApprovalAsync_ReturnsNotFoundResult_WhenModelStateIsValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostApprovalAsync();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnPostApprovalAsync_ReturnsARedirectToPageResult_WhenModelStateIsInValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            pageModel.ModelState.AddModelError("LoanApplication.Purpose", "Loan purpose is required.");
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostApprovalAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostApproveAsync_ReturnsARedirectToPageResult_WhenModelStateIsValid()
        {
            // Arrange
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
            var loanId = 1;
            var loanApplication = await AppDbContext.LoanApplications.AsNoTracking().FirstOrDefaultAsync(m => m.Id == loanId);
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, userManager.Object)
            {
                PageContext = pageContext,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostApproveAsync();

            var approvalApplication = await AppDbContext.LoanApplications.FirstOrDefaultAsync(m => m.Id == loanId);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(LoanStatus.Approved, approvalApplication.Status);
        }

        [Fact]
        public async Task OnPostApproveAsync_ReturnsNotFoundResult_WhenModelStateIsValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostApproveAsync();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnPostApproveAsync_ReturnsARedirectToPageResult_WhenModelStateIsInValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            pageModel.ModelState.AddModelError("LoanApplication.Purpose", "Loan purpose is required.");
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostApproveAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostRejectAsync_ReturnsARedirectToPageResult_WhenModelStateIsValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostRejectAsync();

            var approvalApplication = await AppDbContext.LoanApplications.FirstOrDefaultAsync(m => m.Id == loanId);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(LoanStatus.Rejected, approvalApplication.Status);
        }

        [Fact]
        public async Task OnPostRejectAsync_ReturnsNotFoundResult_WhenModelStateIsValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostRejectAsync();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnPostRejectAsync_ReturnsARedirectToPageResult_WhenModelStateIsInValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            pageModel.ModelState.AddModelError("LoanApplication.Purpose", "Loan purpose is required.");
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostRejectAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostSaveAsync_ReturnsARedirectToPageResult_WhenModelStateIsValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostSaveAsync();

            var approvalApplication = await AppDbContext.LoanApplications.FirstOrDefaultAsync(m => m.Id == loanId);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(LoanStatus.UnderReview, approvalApplication.Status);
        }

        [Fact]
        public async Task OnPostSaveAsync_ReturnsNotFoundResult_WhenModelStateIsValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostSaveAsync();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnPostSaveAsync_ReturnsARedirectToPageResult_WhenModelStateIsInValid()
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
            var applicationReview = await AppDbContext.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == loanId);
            if (applicationReview == null)
            {
                applicationReview = new ApplicationReview
                {
                    ApplicationId = loanId
                };
            }
            var pageModel = new ReviewModel(AppDbContext, authorizationService.Object, mgr.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(actionContext),
                LoanApplication = loanApplication,
                ApplicationReview = applicationReview
            };
            pageModel.ModelState.AddModelError("LoanApplication.Purpose", "Loan purpose is required.");
            // Act
            // A new ModelStateDictionary is valid by default.
            var result = await pageModel.OnPostSaveAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }
    }
}
