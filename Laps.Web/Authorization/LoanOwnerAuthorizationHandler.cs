using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Laps.Web.Authorization
{
    public class LoanOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, LoanApplication>
    {

        UserManager<IdentityUser> _userManager;

        public LoanOwnerAuthorizationHandler(UserManager<IdentityUser>
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, LoanApplication resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for withdraw application, return.
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.WithdrawOperationName)
            {
                return Task.CompletedTask;
            }

            // customer can withdraw application before it is under review
            if (resource.OwnerId == _userManager.GetUserId(context.User) && resource.Status < LoanStatus.UnderReview
                && !context.User.IsInRole(Constants.LoanProcessorsRole) && !context.User.IsInRole(Constants.LoanManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
