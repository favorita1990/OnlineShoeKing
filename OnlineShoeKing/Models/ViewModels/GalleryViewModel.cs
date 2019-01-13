using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoeKing.Models
{
    public class GalleryViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
    }
}