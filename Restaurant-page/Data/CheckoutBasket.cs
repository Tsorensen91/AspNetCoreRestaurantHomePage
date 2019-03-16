using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_page.Data
{
    public class CheckoutBasket
    {
        [StringLength(20)]
        public string Email { get; set; }
        [Key]
        public int BasketID { get; set; }

    }
}
