using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Laps.Web.Data.Models
{
    public class LoanApplication
    {
        public IdentityUser User { get; set; }
        [ForeignKey("IdentityUser")]
        public string OwnerId { get; set; }
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Purpose { get; set; }
        public int Age { get; set; }
        public EmployementType EmploymentType { get; set; }
        public int IncomePerMonth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public long MobileNumber { get; set; }
        public long LoanAmount { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime ApplicationSubmittedOn { get; set; }
        public DateTime StatusLastUpdatedOn { get; set; }
    }
}
