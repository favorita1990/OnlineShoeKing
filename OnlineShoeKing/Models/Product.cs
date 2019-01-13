using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoeKing.Models
{
    public class Product
    {
        public Product()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.Cart = new HashSet<Cart>();
            this.PhotoOfProducts = new HashSet<PhotoOfProduct>(); 
            this.SizeOfProducts = new HashSet<SizeOfProduct>();
            this.Comments = new HashSet<Comment>();
            this.Ratings = new HashSet<Rating>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string BgName { get; set; }
        public decimal? Price { get; set; }
        public bool? Status { get; set; }
        public string PhotoHeader { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool? Specials { get; set; }
        [Range(typeof(decimal), "0", "100")]
        public decimal PromotionPercent { get; set; }
        public string Description { get; set; }
        public string BgDescription { get; set; }
        public bool Deleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }


        public virtual Category Category { get; set; }
        public virtual ICollection<Cart> Cart { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<PhotoOfProduct> PhotoOfProducts { get; set; }
        public virtual ICollection<SizeOfProduct> SizeOfProducts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}