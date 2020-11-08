namespace Laps.Web.Authorization
{
    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";
        public static readonly string WithdrawOperationName = "Withdraw";
        public static readonly string ReviewOperationName = "Review";
        public static readonly string SendForApprovalOperationName = "SendForApproval";

        public static readonly string LoanProcessorsRole = "LoanProcessors";
        public static readonly string LoanManagersRole = "LoanManagers";
    }
}
