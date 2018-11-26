using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Restaurant_page.Pages
{
    public class NewsLetterModel : PageModel
    {
        private AppDbContext _db;
        [BindProperty]
        public Newsletters Newsletters { get; set; }
        public NewsLetterModel(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) { return Page(); }
            _db.Newsletters.Add(Newsletters);
            await _db.SaveChangesAsync();
            return RedirectToPage("/index");
        }
        public void OnGet()
        {

        }
    }
}