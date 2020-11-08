using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace Laps.Web.Authorization
{
    public class LoanCustomerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, LoanApplication>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, LoanApplication resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for approval/reject, return.
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.WithdrawOperationName)
            {
                return Task.CompletedTask;
            }

            // Processors can approve or reject.
            if (!context.User.IsInRole(Constants.LoanProcessorsRole) && !context.User.IsInRole(Constants.LoanManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
