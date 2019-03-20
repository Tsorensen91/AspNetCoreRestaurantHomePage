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
using Stripe;

namespace Restaurant_page.Pages
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        [BindProperty]
        public MenuItem MenuItem { get; set; }
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _UserManager;
        public IList<CheckoutItem> Items { get; private set; }
        public OrderHistory Order = new OrderHistory();
        public decimal OrderTotal = 0;
        public long AmountToPay = 0;

        

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
                "SELECT MenuItems.MenuID, MenuItems.Price, MenuItems.Name, MenuItems.Stock, BasketItems.BasketID, BasketItems.Quantity FROM MenuItems INNER JOIN BasketItems " +
                "ON MenuItems.MenuID = BasketItems.MenuID WHERE BasketID = {0}", customer.BasketID).ToList();


            OrderTotal = 0;
            foreach (var item in Items)
            {
                //checks to see if item has gone out of stock since added to basket
                if (item.Stock <= 0)
                {
                    IList<CheckoutBasketItem> itemsToDelete = _db.BasketItems.FromSql("SELECT * FROM BasketItems WHERE BasketID = {0} and MenuID = {1}", customer.BasketID, item.MenuID).ToList();
                    foreach (var itemToDelete in itemsToDelete)
                    {
                        _db.BasketItems.Remove(itemToDelete);
                        await _db.SaveChangesAsync();
                    }

                } else {
                OrderTotal = OrderTotal + (item.Quantity * item.Price);
                }
            }
            AmountToPay = (long)(OrderTotal * 100);
        }


        //add 1 quantity to item
        public async Task<IActionResult> OnPostAddAsync(int id)
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);

            Items = _db.CheckoutItems.FromSql(
                "SELECT MenuItems.MenuID, MenuItems.Price, MenuItems.Name, MenuItems.Stock, BasketItems.BasketID, BasketItems.Quantity FROM MenuItems INNER JOIN BasketItems " +
                "ON MenuItems.MenuID = BasketItems.MenuID WHERE BasketID = {0}", customer.BasketID).ToList();

            foreach (var item in Items)
            {
                IList<CheckoutBasketItem> itemsToAlter = _db.BasketItems.FromSql("SELECT * FROM BasketItems WHERE MenuID = {0} AND BasketID = {1} ", id, customer.BasketID).ToList();
                foreach (var itemToAlter in itemsToAlter)
                {
                    itemToAlter.Quantity = itemToAlter.Quantity + 1;

                        _db.Attach(itemToAlter).State = EntityState.Modified;
                        try
                        {
                            await _db.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            throw new Exception($"MenuItem {itemToAlter.MenuID} not found!", e);
                        }
                    

                    return RedirectToPage();
                }

            }
            return Page();
        }



        //remove 1 quantity from item
        public async Task<IActionResult> OnPostSubtractAsync(int id)
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);

            Items = _db.CheckoutItems.FromSql(
                "SELECT MenuItems.MenuID, MenuItems.Price, MenuItems.Name, MenuItems.Stock, BasketItems.BasketID, BasketItems.Quantity FROM MenuItems INNER JOIN BasketItems " +
                "ON MenuItems.MenuID = BasketItems.MenuID WHERE BasketID = {0}", customer.BasketID).ToList();

            foreach (var item in Items)
            {
                IList<CheckoutBasketItem> itemsToAlter = _db.BasketItems.FromSql("SELECT * FROM BasketItems WHERE MenuID = {0} AND BasketID = {1} ", id, customer.BasketID).ToList();
                foreach (var itemToAlter in itemsToAlter)
                {
                    itemToAlter.Quantity = itemToAlter.Quantity - 1;

                    if (itemToAlter.Quantity <= 0)
                    {
                            _db.BasketItems.Remove(itemToAlter);
                            await _db.SaveChangesAsync();
                        
                    } else { 

                    _db.Attach(itemToAlter).State = EntityState.Modified;
                    try
                    {
                        await _db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        throw new Exception($"MenuItem {itemToAlter.MenuID} not found!", e);
                    }
                    }

                    return RedirectToPage();
                }
            }

            return Page();
        }

        //pay
        public async Task<IActionResult> OnPostChargeAsync(string stripeEmail, string stripeToken, long amount)
        {
            
            var customers = new CustomerService();
            var charges = new ChargeService();
            var customer = customers.Create(new CustomerCreateOptions{Email = stripeEmail, SourceToken = stripeToken});
            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = amount,
                Description = "J41564 Food Order Charge",
                Currency = "gbp",
                CustomerId = customer.Id
            });

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
            CheckoutCustomer checkoutCustomer = await _db.CheckoutCustomers.FindAsync(user.Email);
            var basketItems = _db.BasketItems.FromSql("SELECT * FROM BasketItems WHERE BasketID = {0}", checkoutCustomer.BasketID).ToList();
            
            foreach (var item in basketItems)
            {
                MenuItem = await _db.MenuItems.FindAsync(item.MenuID);
                MenuItem.Stock -= item.Quantity;
                await _db.SaveChangesAsync();

                Data.OrderItem orderItem = new Data.OrderItem
                {
                    OrderNo = Order.OrderNo,
                    MenuID = item.MenuID,
                    Quantity = item.Quantity
                };
                _db.OrderItems.Add(orderItem);
                _db.BasketItems.Remove(item);
            }

            await _db.SaveChangesAsync();

            return RedirectToPage("/Index", "Payment");
        }


    }
}