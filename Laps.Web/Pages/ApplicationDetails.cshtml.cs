using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace Laps.Web.Pages
{
    [Authorize]
    public class ApplicationDetailsModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public ApplicationDetailsModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public LoanApplication ApplicationDetails { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //ApplicationDetails.Id = Guid.NewGuid().ToString();
            ApplicationDetails.ApplicationSubmittedOn = DateTime.Now;
            ApplicationDetails.StatusLastUpdatedOn = DateTime.Now;
            _context.LoanApplications.Add(ApplicationDetails);
            await _context.SaveChangesAsync();

            return RedirectToPage("./ApplicationsList");
        }
    }
}
