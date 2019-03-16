using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_page.Data
{
    public class MenuItem
    {
        [Key]
        [Required]
        public int MenuID { get; set; }
        [Required, DisplayFormat(DataFormatString ="{0:C2}")]
        public decimal Price { get; set; }
        [Required, StringLength(25)]
        public string Name { get; set; }
        [Required, StringLength(50)]
        public string Description { get; set; }
        [StringLength(100)]
        public string Image { get; set; }

    }
}
