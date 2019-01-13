using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoeKing.Models.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostOffice { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? Total { get; set; }
        public string AdminId { get; set; }
        public string AdminName { get; set; }
        public string OrderStatus { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool Deleted { get; set; }
    }
}