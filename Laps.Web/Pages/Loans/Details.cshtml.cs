using Laps.Web.Data;
using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Laps.Web.Pages.Loans
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public LoanApplication LoanApplication { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LoanApplication = await _context.LoanApplications.FirstOrDefaultAsync(m => m.Id == id);

            if (LoanApplication == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
