using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_page.Data
{
    public class CheckoutCustomer
    {
        [Key]
        [StringLength(20)]
        public string Email { get; set; }
        [Required, StringLength(25)]
        public string Name { get; set; }
        public int BasketID { get; set; }

    }
}
