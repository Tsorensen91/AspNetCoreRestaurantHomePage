using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant_page.Data;
using System.Net.Mail;

namespace Restaurant_page.Pages
{
    public class ContactModel : PageModel
    {

        [BindProperty]
        public ContactUsModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            using (var smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtp.PickupDirectoryLocation = @"c:\maildump";
                var message = new MailMessage
                {
                    Subject = "Message from: " + Input.Name,
                    Body = "Message from: " + Input.Email + " Message: " + Input.Message,
                    From = new MailAddress("theis.soerensen@hotmail.com")
                };
                message.To.Add("theis.soerensen@hotmail.com");
                await smtp.SendMailAsync(message);
            }

            return RedirectToPage("/index");
        }

            public void OnGet()
        {

        }
    }
}