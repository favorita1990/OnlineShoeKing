using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoeKing.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Number { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual UserModel User { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}