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
    public class CheckoutModel : PageModel
    {

        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _UserManager;
        public IList<CheckoutItem> Items { get; private set; }
        public OrderHistory Order = new OrderHistory();

        public CheckoutModel(AppDbContext db, UserManager<ApplicationUser> UserManager)
        {
            _db = db;
            _UserManager = UserManager;
        }


        public async Task OnGetAsync()
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);

            Items = _db.CheckoutItems.FromSql(
                "SELECT MenuItems.MenuID, MenuItems.Price, MenuItems.Name, BasketItems.BasketID, BasketItems.Quantity FROM MenuItems INNER JOIN BasketItems " +
                "ON MenuItems.MenuID = BasketItems.MenuID WHERE BasketID = {0}", customer.BasketID).ToList();
        }

        public async Task<IActionResult> OnPostBuyAsync()
        {
            var currentOrder = _db.OrderHistories.FromSql("SELECT * From OrderHistories").OrderByDescending(b => b.OrderNo).FirstOrDefault();

            if (currentOrder == null)
            {
                Order.OrderNo = 1;
            }
            else
            {
                Order.OrderNo = currentOrder.OrderNo + 1;
            }

            var user = await _UserManager.GetUserAsync(User);
            Order.Email = user.Email;
            _db.OrderHistories.Add(Order);

            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);

            var basketItems =
                _db.BasketItems.FromSql("SELECT * From BasketItems WHERE BasketID = {0}", customer.BasketID).ToList();

            foreach (var item in basketItems)
            {
                OrderItem oi = new OrderItem
                {
                    OrderNo = Order.OrderNo,
                    MenuID = item.MenuID,
                    Quantity = item.Quantity
                };
                _db.OrderItems.Add(oi);
                _db.BasketItems.Remove(item);
            }

            await _db.SaveChangesAsync();

            return RedirectToPage("/Index");
        }


    }
}