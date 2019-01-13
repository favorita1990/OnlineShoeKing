using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineShoeKing.Models
{
    public class HomeSlidesImg
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }
    }
}