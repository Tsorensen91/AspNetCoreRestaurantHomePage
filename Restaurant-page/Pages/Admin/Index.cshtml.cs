using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_page.Data;

namespace Restaurant_page.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IList<MenuItem> MenuItems { get; private set; }
        public IList<OrderHistory> Orders { get; private set; }

        public IndexModel(AppDbContext db, UserManager<ApplicationUser> UserManager)
        {
            _db = db;
        }

        public void OnGet()
        {
            MenuItems = _db.MenuItems.FromSql("SELECT * FROM MenuItems").ToList();
            Orders = _db.OrderHistories.FromSql("SELECT TOP 25  * FROM OrderHistories").ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var itemToDelete = await _db.MenuItems.FindAsync(id);
            if (itemToDelete != null)
            {
                //a check to see if item exists in any previous orders and if so delete it (this sacrifices some book-keeping but allows easy changing of menu
                var orderItems = _db.OrderItems.FromSql("SELECT * FROM OrderItems WHERE MenuID = {0}", id).ToList();

                foreach (var item in orderItems)
                {
                    _db.OrderItems.Remove(item);
                    await _db.SaveChangesAsync();
                }
                //a check to see if item currently in any baskets and if so delete the item in those baskets.
                var basketItems = _db.BasketItems.FromSql("SELECT * FROM BasketItems WHERE MenuID = {0}", id).ToList();

                foreach (var item in basketItems)
                {
                    _db.BasketItems.Remove(item);
                }

                _db.MenuItems.Remove(itemToDelete);
                await _db.SaveChangesAsync();

            }
            return RedirectToPage();
        }
    }
}