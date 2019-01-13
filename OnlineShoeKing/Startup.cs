using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineShoeKing.Startup))]
namespace OnlineShoeKing
{
    using System;
    using System.Linq;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Context;
    using Models;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
            app.MapSignalR();
        }

        private void createRolesandUsers()
        {
            DBContext context = new DBContext();
            var userManager = new UserManager<UserModel>(new UserStore<UserModel>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            const string role = "Founder";
            const string email = "founder1990@abv.bg";
            const string pass = "12";
            if (!roleManager.RoleExists(role))
            {
                roleManager.Create(new IdentityRole(role));
            }

            var PasswordHash = new PasswordHasher();
            if (!context.Users.Any(u => u.Email == email))
            {
                var user = new UserModel
                {
                    Email = email,
                    UserName = email,
                    FirstName = "Iliyan",
                    LastName = "Kodzehamnov",
                    Gender = "0",
                    PasswordHash = PasswordHash.HashPassword(pass),
                    CreationDate = DateTime.Now
                };

                userManager.Create(user);
                userManager.AddToRole(user.Id, role);
            }
            var cart = context.Carts.GroupBy(g => g.CartId).ToList();

            foreach (var items in cart)
            {
                var date = items.FirstOrDefault().DateCreated;
                if (date.Day != DateTime.Now.Day)
                {
                    context.Carts.RemoveRange(items);
                }
            }


            if (!context.OrderStatuses.Any(f => f.Name == "waiting"))
            {
                context.OrderStatuses.Add(new OrderStatus() { Name = "waiting" });
            }
            if (!context.OrderStatuses.Any(f => f.Name == "accepted"))
            {
                context.OrderStatuses.Add(new OrderStatus() { Name = "accepted" });
            }
            if (!context.OrderStatuses.Any(f => f.Name == "canceled"))
            {
                context.OrderStatuses.Add(new OrderStatus() { Name = "canceled" });
            }
            if (!context.OrderStatuses.Any(f => f.Name == "resend"))
            {
                context.OrderStatuses.Add(new OrderStatus() { Name = "resend" });
            }

            if (!context.About.Any())
            {
                var firstAbout = new About()
                                     {
                                         FirstImage = Resources.Pictures.AboutPicFirst,
                                         SecondImage = Resources.Pictures.AboutPicSecond,
                                         FirstTextHeader = Resources.Language.PartnershipsWordTitle,
                                         SecondTextHeader = Resources.Language.PartnershipsWordTitle,
                                         FirstText = Resources.Language.PartnershipWithUNICEFWordsTitle,
                                         SecondText = Resources.Language.Beginnings21thCenturyTitle
                                     };

                context.About.Add(firstAbout);
            }

            if (!context.HomePage.Any())
            {
                var homePage = new HomePage()
                                     {
                                         TextHeader = Resources.Language.HomePageHeaderText,
                                         Text = Resources.Language.HomePageText,
                                         ImageUrl = Resources.Pictures.HomePagePicture
                                    };

                context.HomePage.Add(homePage);
            }

            context.SaveChanges();
        }
    }
}
