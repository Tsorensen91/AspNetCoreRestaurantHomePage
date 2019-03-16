using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_page.Data;

namespace Restaurant_page.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public GuestLogin Input { get; set; }
        public CheckoutCustomer Customer = new CheckoutCustomer();
        public CheckoutBasket Basket = new CheckoutBasket();

        private AppDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public LoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _signInManager.PasswordSignInAsync(Input.Email, Input.LoginPassword, false, lockoutOnFailure: false);
                if (loginResult.Succeeded)
                {
                    return LocalRedirect("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {

            if (ModelState.IsValid)
            {
                var guest = new ApplicationUser { UserName = Input.RegisterEmail, Email = Input.RegisterEmail };
                var result = await _userManager.CreateAsync(guest, Input.RegisterPassword);

                if (result.Succeeded)
                {
                    var newGuest = await _userManager.FindByEmailAsync(Input.RegisterEmail);
                    var setRole = await _userManager.AddToRoleAsync(newGuest, "Member");
                    await _signInManager.SignInAsync(guest, isPersistent: false);

                    NewCheckoutBasket(Input.RegisterEmail);
                    NewCustomer(Input.RegisterEmail);
                    await _db.SaveChangesAsync();

                    return LocalRedirect("/Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

        public void NewCheckoutBasket(string Email)
        {
            Basket.Email = Email;
            var currentBasket = _db.Baskets.FromSql("SELECT * From Baskets")
                .OrderByDescending(b => b.BasketID)
                .FirstOrDefault();
            if (currentBasket == null)
            {
                Basket.BasketID = 1;
            }
            else
            {
                Basket.BasketID = currentBasket.BasketID + 1;
            }

            _db.Baskets.Add(Basket);
        }

        public void NewCustomer(string Email)
        {
            Customer.Email = Email;
            Customer.BasketID = Basket.BasketID;
            _db.CheckoutCustomers.Add(Customer);
        }
    }
}