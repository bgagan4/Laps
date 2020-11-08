using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Laps.Web.Authorization
{
    public static class LoanOperations
    {
        public static OperationAuthorizationRequirement Create =
          new OperationAuthorizationRequirement { Name = Constants.CreateOperationName };
        public static OperationAuthorizationRequirement Read =
          new OperationAuthorizationRequirement { Name = Constants.ReadOperationName };
        public static OperationAuthorizationRequirement Update =
          new OperationAuthorizationRequirement { Name = Constants.UpdateOperationName };
        public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };
        public static OperationAuthorizationRequirement Approve =
          new OperationAuthorizationRequirement { Name = Constants.ApproveOperationName };
        public static OperationAuthorizationRequirement Reject =
          new OperationAuthorizationRequirement { Name = Constants.RejectOperationName };
        public static OperationAuthorizationRequirement Withdraw =
          new OperationAuthorizationRequirement { Name = Constants.WithdrawOperationName };
        public static OperationAuthorizationRequirement Review =
          new OperationAuthorizationRequirement { Name = Constants.ReviewOperationName };
        public static OperationAuthorizationRequirement SendForApproval =
          new OperationAuthorizationRequirement { Name = Constants.SendForApprovalOperationName };
    }
}
