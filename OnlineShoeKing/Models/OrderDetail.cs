using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineShoeKing.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public int SizeId { get; set; }
        public int? Quantity { get; set; }
        public bool? Status { get; set; }
        public bool? Specials { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }


        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}