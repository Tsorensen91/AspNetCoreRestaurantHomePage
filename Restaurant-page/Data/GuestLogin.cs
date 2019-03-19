using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_page
{
    public class GuestLogin
    {
        [EmailAddress, Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100), DataType(DataType.Password), Display(Name = "Password")]
        public string LoginPassword { get; set; }

    }
}

