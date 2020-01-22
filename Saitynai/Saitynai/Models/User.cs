using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Saitynai.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Sex { get; set; }
        public DateTime BirthDate { get; set; }


        //public virtual ICollection<Order> Orders { get; set; }
    }
}
