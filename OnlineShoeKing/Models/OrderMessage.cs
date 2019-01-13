using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineShoeKing.Models
{
    public class OrderMessage
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Added { get; set; }
        public bool Deleted { get; set; }
        public bool ReadOrUnread { get; set; }

        [ForeignKey("Order")]
        public Nullable<int> OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}