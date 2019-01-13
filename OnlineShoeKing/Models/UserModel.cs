using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OnlineShoeKing.Models
{

    public class UserModel : IdentityUser
    {
        public UserModel()
        {
            this.Galleries = new HashSet<Gallery>();
            this.Messages = new HashSet<Message>();
            this.Orders = new HashSet<Order>();
            this.Comments = new HashSet<Comment>();
            this.Ratings = new HashSet<Rating>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime CreationDate { get; set; }
        public string ImageUrl { get; set; }
        public string CoverUrl { get; set; }
        public string ConnectionId { get; set; }
        public bool OnlineOrOffline { get; set; }
        public string ChattingWithUserId { get; set; }
        public bool? ProfileMainStatus { get; set; }
        public bool? ProfilePhotosStatus { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserModel> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual ICollection<Gallery> Galleries { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }

    }
}