using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_page
{
    public class Newsletters
    { 
        [Key][Required, StringLength(40)]
        public string Email { get; set; }
        [Required, StringLength(10)]
        public string Name { get; set; }
    }
}
