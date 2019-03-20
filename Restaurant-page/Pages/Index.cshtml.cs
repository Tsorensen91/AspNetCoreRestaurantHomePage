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
        public string Arrival = "";

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            MenuItems = _db.MenuItems.FromSql("SELECT TOP 3 * FROM MenuItems WHERE Stock > 0").ToList();
        }

        public void OnGetLogin()
        {
            MenuItems = _db.MenuItems.FromSql("SELECT TOP 3 * FROM MenuItems WHERE Stock > 0").ToList();
            Arrival = "login";
        }

        public void OnGetPayment()
        {
            MenuItems = _db.MenuItems.FromSql("SELECT TOP 3 * FROM MenuItems WHERE Stock > 0").ToList();
            Arrival = "payment";
        }

        public void OnGetContact()
        {
            MenuItems = _db.MenuItems.FromSql("SELECT TOP 3 * FROM MenuItems WHERE Stock > 0").ToList();
            Arrival = "contact";
        }

        public void OnGetLogout()
        {
            MenuItems = _db.MenuItems.FromSql("SELECT TOP 3 * FROM MenuItems WHERE Stock > 0").ToList();
            Arrival = "logout";
        }
    }
}