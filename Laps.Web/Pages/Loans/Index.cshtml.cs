using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Laps.Web.Data;
using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Laps.Web.Authorization;

namespace Laps.Web.Pages.Loans
{
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IList<LoanApplication> LoanApplications { get; set; }

        public async Task OnGetAsync(bool showAll = false)
        {
            var loanApplications = from l in Context.LoanApplications
                                   select l;

            var isProcessor = User.IsInRole(Constants.LoanProcessorsRole);
            var isManager = User.IsInRole(Constants.LoanManagersRole);
            var isCustomer = !isProcessor && !isManager;

            if (showAll && isManager)
            {
                LoanApplications = await loanApplications.ToListAsync();
                return;
            }

            var currentUserId = UserManager.GetUserId(User);

            if (isCustomer)
            {
                loanApplications = loanApplications.Where(l => l.OwnerId == currentUserId);
            }
            else if (isProcessor)
            {
                loanApplications = loanApplications.Where(l => l.Status < LoanStatus.AwaitingManagersApproval);
            }
            else if (isManager)
            {
                loanApplications = loanApplications.Where(l => l.Status == LoanStatus.AwaitingManagersApproval);
            }

            LoanApplications = await loanApplications.ToListAsync();
        }
    }
}
