using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laps.Web.Data;
using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using Laps.Web.Authorization;

namespace Laps.Web.Pages.Loans
{
    public class WithdrawModel : DI_BasePageModel
    {
        public WithdrawModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        public LoanApplication LoanApplication { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LoanApplication = await Context.LoanApplications.FirstOrDefaultAsync(m => m.Id == id);

            if (LoanApplication == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                 User, LoanApplication,
                                                 LoanOperations.Withdraw);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LoanApplication = await Context.LoanApplications.FindAsync(id);

            if (LoanApplication == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                 User, LoanApplication,
                                                 LoanOperations.Withdraw);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }


            if (LoanApplication != null)
            {
                LoanApplication.Status = LoanStatus.Withdrawn;
                LoanApplication.StatusLastUpdatedOn = DateTime.Now;
                Context.Attach(LoanApplication).State = EntityState.Modified;
                await Context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
