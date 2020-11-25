using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Laps.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<ApplicationReview> ApplicationReviews { get; set; }

        public async virtual Task AddLoanApplicationAsync(LoanApplication loanApplication)
        {
            await LoanApplications.AddAsync(loanApplication);
            await SaveChangesAsync();
        }
    }
}
