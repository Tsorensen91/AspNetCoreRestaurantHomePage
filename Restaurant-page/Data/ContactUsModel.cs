using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_page.Data
{
    public class ContactUsModel
    {
        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required, StringLength(250)]
        public string Message { get; set; }

    }
}
