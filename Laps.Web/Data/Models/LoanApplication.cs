using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laps.Web.Data.Models
{
    public class LoanApplication
    {
        public IdentityUser User { get; set; }
        [ForeignKey("IdentityUser")]
        public string OwnerId { get; set; }
        public int Id { get; set; }
        public string ExternalId { get; set; }
        [Required]
        public string Purpose { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public EmployementType EmploymentType { get; set; }
        [Required]
        public int IncomePerMonth { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Zip { get; set; }
        [Required]
        public long MobileNumber { get; set; }
        [Required]
        public long LoanAmount { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime ApplicationSubmittedOn { get; set; }
        public DateTime StatusLastUpdatedOn { get; set; }
    }
}
