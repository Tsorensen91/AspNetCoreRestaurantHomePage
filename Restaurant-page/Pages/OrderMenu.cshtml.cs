using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_page.Data;

namespace Restaurant_page.Pages
{
    public class OrderMenuModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _UserManager;
        public IList<MenuItem> MenuItems { get; private set; }

        public OrderMenuModel(AppDbContext db, UserManager<ApplicationUser> UserManager)
        {
            _db = db;
            _UserManager = UserManager;
        }

        public void OnGet()
        {
            MenuItems = _db.MenuItems.FromSql("SELECT * FROM MenuItems").ToList();
        }

        public async Task<IActionResult> OnPostBuyAsync(int id)
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);
            var item = _db.BasketItems.FromSql("SELECT * FROM BasketItems WHERE MenuID = {0} " +
            "AND BasketID = {1}", id, customer.BasketID).ToList().FirstOrDefault();

            if (item == null)
            {
                CheckoutBasketItem newItem = new CheckoutBasketItem
                {
                    BasketID = customer.BasketID,
                    MenuID = id,
                    Quantity = 1
                };
                _db.BasketItems.Add(newItem);
                await _db.SaveChangesAsync();
            }
            else
            {
                item.Quantity = item.Quantity + 1;
                _db.Attach(item).State = EntityState.Modified;
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    throw new Exception($"Basket not found!", e);
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var itemToDelete = await _db.MenuItems.FindAsync(id);
            if (itemToDelete != null)
            {
                //a check to see if item currently in any baskets and if so delete the item in those baskets.
                var basketItems = _db.BasketItems.FromSql("SELECT * FROM BasketItems WHERE MenuID = {0}", id).ToList();
                foreach(var item in basketItems)
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