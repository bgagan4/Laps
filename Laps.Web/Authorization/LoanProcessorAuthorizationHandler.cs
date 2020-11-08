using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace Laps.Web.Authorization
{
    public class LoanProcessorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, LoanApplication>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, LoanApplication resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for approval/reject, return.
            if (requirement.Name != Constants.SendForApprovalOperationName &&
                requirement.Name != Constants.RejectOperationName &&
                requirement.Name != Constants.ReviewOperationName)
            {
                return Task.CompletedTask;
            }

            // Processors can approve or reject.
            if (context.User.IsInRole(Constants.LoanProcessorsRole) && resource.Status < LoanStatus.AwaitingManagersApproval)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
