using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Saitynai.Models
{
    public class Order
    {
        [Key] public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public double Price { get; set; }

        public string UserId { get; set; }
        //public virtual ICollection<OrderProduct> OrderProduct { get; set; }
    }
}
