using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineShoeKing.Models
{
    public class Gallery
    {
        [Key]
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string ImageUrl { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual UserModel User { get; set; }
    }
}