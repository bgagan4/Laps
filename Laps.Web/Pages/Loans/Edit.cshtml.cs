using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laps.Web.Data;
using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Laps.Web.Authorization;

namespace Laps.Web.Pages.Loans
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(
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

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, LoanApplication, LoanOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }


            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch Application from DB to get OwnerID.
            var loanApplication = await Context
                .LoanApplications.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (loanApplication == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, loanApplication,
                                                     LoanOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            LoanApplication.OwnerId = loanApplication.OwnerId;
            LoanApplication.ExternalId = loanApplication.ExternalId;
            LoanApplication.ApplicationSubmittedOn = loanApplication.ApplicationSubmittedOn;
            LoanApplication.StatusLastUpdatedOn = loanApplication.StatusLastUpdatedOn;
            Context.Attach(LoanApplication).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanApplicationExists(LoanApplication.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LoanApplicationExists(int id)
        {
            return Context.LoanApplications.Any(e => e.Id == id);
        }
    }
}
