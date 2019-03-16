using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_page.Data
{
    public class CheckoutBasketItem
    {
        [Required]
        public int MenuID { get; set; }
        [Required]
        public int BasketID { get; set; }
        [Required, StringLength(50)]
        public int Quantity { get; set; }

    }
}
