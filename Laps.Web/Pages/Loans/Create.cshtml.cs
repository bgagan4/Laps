using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Laps.Web.Data;
using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Laps.Web.Authorization;

namespace Laps.Web.Pages.Loans
{
    public class CreateModel : DI_BasePageModel
    {

        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            LoanApplication = new LoanApplication { OwnerId = UserManager.GetUserId(User) };
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, LoanApplication,
                                                        LoanOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            return Page();
        }

        [BindProperty]
        public LoanApplication LoanApplication { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            LoanApplication.OwnerId = UserManager.GetUserId(User);
            LoanApplication.ApplicationSubmittedOn = DateTime.Now;
            LoanApplication.StatusLastUpdatedOn = DateTime.Now;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, LoanApplication,
                                                        LoanOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }


            LoanApplication.ExternalId = Guid.NewGuid().ToString();
            await Context.AddLoanApplicationAsync(LoanApplication);

            return RedirectToPage("./Index");
        }
    }
}
