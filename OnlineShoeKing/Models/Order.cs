using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineShoeKing.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.OrderMessages = new HashSet<OrderMessage>();
        }

        [Key]
        public int Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostOffice { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? Total { get; set; }
        public string AdminId { get; set; }
        public int OrderStatusId { get; set; }
        public bool Deleted { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual UserModel User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<OrderMessage> OrderMessages { get; set; }
    }
}