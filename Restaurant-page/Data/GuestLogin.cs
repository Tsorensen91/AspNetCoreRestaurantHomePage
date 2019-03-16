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

        [EmailAddress, Display(Name = "Email")]
        public string RegisterEmail { get; set; }

        [StringLength(100), DataType(DataType.Password), Display(Name = "Password")]
        public string LoginPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6), DataType(DataType.Password), Display(Name = "Password")]
        public string RegisterPassword { get; set; }

        [DataType(DataType.Password), Display(Name = "Confirm password"), Compare("RegisterPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmregisterPassword { get; set; }
    }
}

