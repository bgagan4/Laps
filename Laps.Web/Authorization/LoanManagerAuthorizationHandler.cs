using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace Laps.Web.Authorization
{
    public class LoanManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, LoanApplication>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, LoanApplication resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.ReviewOperationName &&
                requirement.Name != Constants.SendForApprovalOperationName &&
                requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Constants.LoanManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
