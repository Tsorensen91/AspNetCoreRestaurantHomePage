using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_page
{
    public class Guest
    {
        [Key]
        public int ID { get; set; }
        [Required, StringLength(10)]
        public string Name { get; set; }
        
    }
}
