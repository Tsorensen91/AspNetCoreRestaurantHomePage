﻿using System;
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
                "SELECT MenuItems.MenuID, MenuItems.Price, MenuItems.Name, BasketItems.BasketID, BasketItems.Quantity FROM MenuItems INNER JOIN BasketItems " +
                "ON MenuItems.MenuID = BasketItems.MenuID WHERE BasketID = {0}", customer.BasketID).ToList();

            OrderTotal = 0;
            foreach (var item in Items)
            {
                OrderTotal = OrderTotal + (item.Quantity * item.Price);
            }
            AmountToPay = (long)(OrderTotal * 100);

        }


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
            var basketItems = _db.BasketItems.FromSql("SELECT * From BasketItems WHERE BasketID = {0}", checkoutCustomer.BasketID).ToList();
            foreach (var item in basketItems)
            {
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

            return RedirectToPage("/Index");
        }


    }
}