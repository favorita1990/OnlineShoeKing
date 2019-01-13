using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoeKing.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public string CreationDate { get; set; }
        public string Gender { get; set; }
        public string ConnectionId { get; set; }
        public bool OnlineOrOffline { get; set; }
        public string ChattingWithUserId { get; set; }

    }
}