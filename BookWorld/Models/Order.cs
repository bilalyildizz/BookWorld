using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookWorld.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string MusteriId { get; set; }
        [ForeignKey("MusteriId")]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime? OrderDate { get; set; }
        public int? TotalAmount { get; set; }
        public bool OrderSituation { get; set; } = false;

    }
}
