using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_page.Data
{
    public class CheckoutItem
    {

        [Key, Required]
        public int MenuID { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required, StringLength(25)]
        public string Name { get; set; }
        [Required]
        public int Quantity { get; set; }

    }
}
