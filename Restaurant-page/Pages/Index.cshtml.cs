using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_page.Data;

namespace Restaurant_page.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IList<MenuItem> MenuItems { get; private set; }

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            MenuItems = _db.MenuItems.FromSql("SELECT * FROM MenuItems").ToList();
        }
    }
}