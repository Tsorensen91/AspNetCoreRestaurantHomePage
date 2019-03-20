using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant_page.Data;
using System.Net;
using System.Net.Mail;

namespace Restaurant_page.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty]
        public ContactUsModel Input { get; set; }

        public IActionResult OnPost()
        {
            var mailbody = $@"Contact message recieved from Abra Kebabra:

                            Name: {Input.Name}
                            Email: {Input.Email}
                            Message: ""{Input.Message}""";
            SendMail(mailbody);

            return RedirectToPage("/Index", "contact");
        }

        private void SendMail(string mailbody)
        {
            //adapted from Ben. (2012). Retrieved from https://stackoverflow.com/questions/13506623/smtp-exception-failure-sending-mail.
            NetworkCredential loginInfo = new NetworkCredential("j41564chester@gmail.com", "1721102chester");
            using (var message = new MailMessage(Input.Email, "j41564chester@gmail.com"))
            {
                message.To.Add(new MailAddress("j41564chester@gmail.com"));
                message.From = new MailAddress(Input.Email);
                message.Subject = "Message from Abra Kebabra";
                message.Body = mailbody;

                using (var smtpClient = new SmtpClient("j41564chester@gmail.com"))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = loginInfo;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.EnableSsl = true;
                    smtpClient.Port = 587;
                    smtpClient.Send(message);
                }
            }
        }

    }
}