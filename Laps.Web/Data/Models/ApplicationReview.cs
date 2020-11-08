using System.ComponentModel.DataAnnotations.Schema;

namespace Laps.Web.Data.Models
{
    public class ApplicationReview
    {
        public int Id { get; set; }
        public LoanApplication Application { get; set; }
        [ForeignKey("LoanApplication")]
        public int ApplicationId { get; set; }
        [ForeignKey("IdentityUser")]
        public string ReviewerId { get; set; }
        public bool IsIncomeVerified { get; set; }
        public bool IsCreditScoreVerified { get; set; }
    }
}
