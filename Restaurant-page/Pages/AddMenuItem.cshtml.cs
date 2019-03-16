using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant_page.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Restaurant_page.Pages
{
    public class AddMenuItemModel : PageModel
    {
        private AppDbContext _db;
        private readonly IHostingEnvironment _he;
        [BindProperty]
        public MenuItem MenuItem { get; set; }
        [BindProperty]
        public IFormFile ItemImage { get; set; }

        public AddMenuItemModel(AppDbContext db, IHostingEnvironment he)
        {
            _db = db;
            _he = he;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { return Page(); }

            if (ItemImage != null)
            {
                var filename = Path.Combine(_he.WebRootPath, "images", Path.GetFileName(ItemImage.FileName));
                MenuItem.Image = Path.Combine("images", Path.GetFileName(ItemImage.FileName));
                ItemImage.CopyTo(new FileStream(filename, FileMode.Create));
            }

            _db.MenuItems.Add(MenuItem);
            await _db.SaveChangesAsync();
            return RedirectToPage("/Menu");
        }

        public void OnGet()
        {

        }
    }
}