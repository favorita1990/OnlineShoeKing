using System;
using System.Collections.Generic;

namespace OnlineShoeKing.Models
{
    public class Category
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string BgName { get; set; }
        public string Photo { get; set; }
        public bool Status { get; set; }
        public bool? Gender { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}