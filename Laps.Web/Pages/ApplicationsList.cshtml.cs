using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laps.Web.Pages
{
    [Authorize]
    public class ApplicationsListModel : PageModel
    {
        private readonly Laps.Web.Data.ApplicationDbContext _context;

        public ApplicationsListModel(Laps.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<LoanApplication> ApplicationDetails { get; set; }

        public async Task OnGetAsync()
        {
            ApplicationDetails = await _context.LoanApplications
                .Include(a => a.User).ToListAsync();
        }
    }
}
