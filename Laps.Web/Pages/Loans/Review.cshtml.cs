using System;
using System.Linq;
using System.Threading.Tasks;
using Laps.Web.Authorization;
using Laps.Web.Data;
using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Laps.Web.Pages.Loans
{
    public class ReviewModel : DI_BasePageModel
    {
        public ReviewModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
        {
        }

        public LoanApplication LoanApplication { get; set; }
        [BindProperty]
        public ApplicationReview ApplicationReview { get; set; }

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

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, LoanApplication, LoanOperations.Review);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            ApplicationReview = await Context.ApplicationReviews.FirstOrDefaultAsync(m => m.Application.Id == id);
            if(ApplicationReview == null)
            {
                ApplicationReview = new ApplicationReview
                {
                    ApplicationId = LoanApplication.Id
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApprovalAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch Application from DB to get OwnerID.
            var loanApplication = await Context
                .LoanApplications.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == ApplicationReview.ApplicationId);

            if (loanApplication == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, loanApplication,
                                                     LoanOperations.SendForApproval);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            //var isIncomeChecked = loanApplication.IsIncomeVerified;
            //var isCreditChecked = loanApplication.IsCreditScoreVerified;

            LoanApplication = loanApplication;
            LoanApplication.Status = LoanStatus.AwaitingManagersApproval;
            LoanApplication.StatusLastUpdatedOn = DateTime.Now;
            //LoanApplication.IsIncomeVerified = isIncomeChecked;
            //LoanApplication.IsCreditScoreVerified = isCreditChecked;
            Context.Attach(LoanApplication).State = EntityState.Modified;

            if(ApplicationReview.Id == 0)
            {
                ApplicationReview.ReviewerId = UserManager.GetUserId(User);
                ApplicationReview.Application = LoanApplication;
                Context.ApplicationReviews.Add(ApplicationReview);
            }
            else
            {
                Context.Attach(ApplicationReview).State = EntityState.Modified;
            }

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

        public async Task<IActionResult> OnPostApproveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch Application from DB to get OwnerID.
            var loanApplication = await Context
                .LoanApplications.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == ApplicationReview.ApplicationId);

            if (loanApplication == null)
            {
                return NotFound();
            }

            var isAuthorized = User.IsInRole(Constants.LoanManagersRole);
            if (!isAuthorized)
            {
                return Forbid();
            }

            LoanApplication = loanApplication;
            LoanApplication.Status = LoanStatus.Approved;
            LoanApplication.StatusLastUpdatedOn = DateTime.Now;
            Context.Attach(LoanApplication).State = EntityState.Modified;

            if (ApplicationReview.Id == 0)
            {
                ApplicationReview.ReviewerId = UserManager.GetUserId(User);
                ApplicationReview.Application = LoanApplication;
                Context.ApplicationReviews.Add(ApplicationReview);
            }
            else
            {
                Context.Attach(ApplicationReview).State = EntityState.Modified;
            }

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

        public async Task<IActionResult> OnPostRejectAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch Application from DB to get OwnerID.
            var loanApplication = await Context
                .LoanApplications.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == ApplicationReview.ApplicationId);

            if (loanApplication == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, loanApplication,
                                                     LoanOperations.Reject);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            //var isIncomeChecked = loanApplication.IsIncomeVerified;
            //var isCreditChecked = loanApplication.IsCreditScoreVerified;

            LoanApplication = loanApplication;
            LoanApplication.Status = LoanStatus.Rejected;
            LoanApplication.StatusLastUpdatedOn = DateTime.Now;
            //LoanApplication.IsIncomeVerified = isIncomeChecked;
            //LoanApplication.IsCreditScoreVerified = isCreditChecked;
            Context.Attach(LoanApplication).State = EntityState.Modified;

            if (ApplicationReview.Id == 0)
            {
                ApplicationReview.ReviewerId = UserManager.GetUserId(User);
                ApplicationReview.Application = LoanApplication;
                Context.ApplicationReviews.Add(ApplicationReview);
            }
            else
            {
                Context.Attach(ApplicationReview).State = EntityState.Modified;
            }

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

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch Application from DB to get OwnerID.
            var loanApplication = await Context
                .LoanApplications.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == ApplicationReview.ApplicationId);

            if (loanApplication == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, loanApplication,
                                                     LoanOperations.Review);
            if (!isAuthorized.Succeeded )
            {
                return Forbid();
            }

            LoanApplication = loanApplication;
            LoanApplication.Status = LoanStatus.UnderReview;
            LoanApplication.StatusLastUpdatedOn = DateTime.Now;
            Context.Attach(LoanApplication).State = EntityState.Modified;

            if (ApplicationReview.Id == 0)
            {
                ApplicationReview.ReviewerId = UserManager.GetUserId(User);
                ApplicationReview.Application = LoanApplication;
                Context.ApplicationReviews.Add(ApplicationReview);
            }
            else
            {
                Context.Attach(ApplicationReview).State = EntityState.Modified;
            }

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

            TempData["SuccessMessage"] = "Review updated succesfully!";
            return RedirectToPage(new { id = LoanApplication.Id });
        }

        private bool LoanApplicationExists(int id)
        {
            return Context.LoanApplications.Any(e => e.Id == id);
        }
    }
}
