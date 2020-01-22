using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Saitynai.Models
{
    public class Product
    {
        [Key] public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }

        public string Description { get; set; }

        //public virtual ICollection<OrderProduct> OrderProduct { get; set; }
    }
}
