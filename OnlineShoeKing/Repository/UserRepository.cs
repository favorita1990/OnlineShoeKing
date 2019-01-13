
namespace OnlineShoeKing.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Context;
    using Models;

    public class UserRepository
    {
        public DBContext db = new DBContext();

        public string FirstName(string name)
        {
            try
            {
                return db.Users.Where(w => w.Email == name).Select(s => s.FirstName).ToList().Last();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string UserId(string email)
        {
            try
            {
                return db.Users.Where(w => w.Email == email).Select(s => s.Id).ToList().Last();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public UserModel GetUserById(string userId)
        {

            return db.Users.FirstOrDefault(f => f.Id == userId);
        }

        public UserModel GetUserByEmail(string username)
        {

            return db.Users.FirstOrDefault(f => f.UserName == username);
        }

        public List<UserViewModel> GetUsersByProductId(string currentUserId, int productId)
        {
            var users = new List<UserViewModel>();
            var orders = this.db.Orders?.OrderByDescending(d => d.Created).ToList();

            foreach (var order in orders)
            {
                var orderUser = order.OrderDetails.FirstOrDefault(f => f.ProductId == productId);
                if (orderUser != null && ((currentUserId == null && users.Count(x => x.Email != order.User.Email) == users.Count) || (!users.Any() && currentUserId == null)))
                {

                    var user = new UserViewModel
                                   {
                                       Email = order.User.Email,
                                       CreationDate = order.User.CreationDate.ToString("dd.MM.yyyy"),
                                       FirstName = order.User.FirstName,
                                       LastName = order.User.LastName,
                                       FullName = $"{order.User.FirstName} {order.User.LastName}",
                                       Gender = order.User.Gender,
                                       ImageUrl = order.User.ImageUrl != null
                                                      ? Resources.Pictures.FolderUsersImages
                                                        + order.User.ImageUrl
                                                      : (order.User.Gender == "1"
                                                             ? Resources.Pictures.ProfileWoman
                                                             : Resources.Pictures.ProfilePic)
                                   };

                    users.Add(user);
                }
                else if (orderUser != null && ((users.Count(x => x.Email != order.User.Email) == users.Count && order.User.Id != currentUserId) || 
                                          (!users.Any() && order.User.Id != currentUserId)))
                {

                    var user = new UserViewModel
                                   {
                                       Email = order.User.Email,
                                       CreationDate = order.User.CreationDate.ToString("dd.MM.yyyy"),
                                       FirstName = order.User.FirstName,
                                       LastName = order.User.LastName,
                                       FullName = $"{order.User.FirstName} {order.User.LastName}",
                                       Gender = order.User.Gender,
                                       ImageUrl = order.User.ImageUrl != null
                                                      ? Resources.Pictures.FolderUsersImages
                                                        + order.User.ImageUrl
                                                      : (order.User.Gender == "1"
                                                             ? Resources.Pictures.ProfileWoman
                                                             : Resources.Pictures.ProfilePic)
                                   };

                    users.Add(user);
                }
            }

            return users;
        }

        public List<UserViewModel> GetAllCustomers(string userId)
        {
            var usersInRole = db.Users.Where(m => m.Id != userId);

            var users = new List<UserViewModel>();
            foreach (var user in usersInRole)
            {
                users.Add(
                    new UserViewModel
                    {
                            Email = user.Email,
                            CreationDate = user.CreationDate.ToString("dd.MM.yyyy"),
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            FullName = $"{user.FirstName} {user.LastName}",
                            Gender = user.Gender,
                            ImageUrl = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl : 
                                           (user.Gender == "1" ? Resources.Pictures.ProfileWoman : Resources.Pictures.ProfilePic),
                            OnlineOrOffline = user.OnlineOrOffline
                        });
            }

            return users;
        }

        public List<UserViewModel> SortAllCustomers(string userId, string sortBy)
        {
            var usersInRole = db.Users.Where(m => m.Id != userId);

            switch (sortBy)
            {
                case "1":
                    usersInRole = usersInRole.OrderBy(o => o.FirstName); break;
                case "-1":
                    usersInRole = usersInRole.OrderByDescending(o => o.FirstName); break;
                case "2":
                    usersInRole = usersInRole.OrderBy(o => o.LastName); break;
                case "-2":
                    usersInRole = usersInRole.OrderByDescending(o => o.LastName); break;
                case "3":
                    usersInRole = usersInRole.OrderBy(o => o.CreationDate); break;
                case "-3":
                    usersInRole = usersInRole.OrderByDescending(o => o.CreationDate); break;
                case "4":
                    usersInRole = usersInRole.OrderBy(o => o.Gender); break;
                case "-4":
                    usersInRole = usersInRole.OrderByDescending(o => o.Gender); break;
            }

            var users = new List<UserViewModel>();
            foreach (var user in usersInRole)
            {
                users.Add(
                    new UserViewModel
                        {
                            Email = user.Email,
                            CreationDate = user.CreationDate.ToString("dd.MM.yyyy"),
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            FullName = $"{user.FirstName} {user.LastName}",
                            Gender = user.Gender,
                            ImageUrl = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                           (user.Gender == "1" ? Resources.Pictures.ProfileWoman : Resources.Pictures.ProfilePic),
                            OnlineOrOffline = user.OnlineOrOffline
                        });
            }


            return users;
        }

        public string UserCurrentPhone(string email)
        {
            try
            {
                return db.Users.Where(w => w.UserName == email).Select(s => s.PhoneNumber).ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void ChangeProfileMainStatus(string userId, int radioVal)
        {
            var profileStatus = radioVal == 0 ? false : true;
            var user = this.db.Users.Find(userId);
            user.ProfileMainStatus = profileStatus;
            this.db.SaveChanges();
        }

        public void ChangeProfilePhotosStatus(string userId, int radioVal)
        {
            var profileStatus = radioVal == 0 ? false : true;
            var user = this.db.Users.Find(userId);
            user.ProfilePhotosStatus = profileStatus;
            this.db.SaveChanges();
        }

        public string FlowerUserRoleName(string email)
        {
            try
            {
                //var role = (from r in db.Roles where r.Name.Contains("Admin") select r).FirstOrDefault();
                //var users = db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(role.Id)).ToList();
                //if (users.Find(x => x.Id == userId) != null)

                var user = db.Users.First(u => u.Email == email);
                string userId = user.Id;

                
                var userRoles = db.Roles.Include(r => r.Users).ToList();

                var userRoleNames = (from r in userRoles
                                     from u in r.Users
                                     where u.UserId == userId
                                     select r.Name).FirstOrDefault();
               
               
                if (userRoleNames != null)
                {
                    return userRoleNames;
                }
                else
                {

                    return "";
                }

                
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}