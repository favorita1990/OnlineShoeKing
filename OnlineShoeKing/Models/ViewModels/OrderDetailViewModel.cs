using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoeKing.Models.ViewModels
{
    public class OrderDetailViewModel
    {
        public int OrderDetailsId { get; set; }
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public bool? Status { get; set; }
        public bool? Specials { get; set; }
        public int OrderId { get; set; }
    }
}