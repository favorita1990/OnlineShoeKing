

namespace OnlineShoeKing.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Context;
    using Models;

    using Repository;

    [Authorize(Roles = ("Admin"))]
    public class CustomersController : Controller
    {
        public DBContext db = new DBContext();

        public readonly UserRepository userRepository = new UserRepository();

        private readonly BreadCrumbRepository breadCrumbRepository = new BreadCrumbRepository();

        [HttpPost]
        public ActionResult SortAllCustomers(string sortBy)
        {

            CurrentSession.SortBy = CurrentSession.SortBy == sortBy ? $"-{sortBy}" : sortBy;

            var userId = User.Identity.GetUserId();

            var users = userRepository.SortAllCustomers(userId, CurrentSession.SortBy);

            var controllerName = Resources.Language.Customers;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            TempData["sortBy"] = sortBy;

            return this.PartialView("_Index", users);
        }

        [HttpPost]
        public JsonResult GetPhotosAlbum(string email)
        {
            var user = db.Users.FirstOrDefault(w => w.Email == email);
            var photosAlbum = db.Galleries.Where(w => w.UserId == user.Id).ToList();
            photosAlbum = photosAlbum.OrderByDescending(o => o.Time).ToList();
            var gallery = new List<GalleryViewModel>();
            foreach (var item in photosAlbum)
            {
                gallery.Add(new GalleryViewModel
                {
                    Id = item.Id,
                    ImageUrl = Resources.Pictures.FolderUsersImages + item.ImageUrl,
                    UserId = user.Id,
                    UserName = user.Email
                });
            }

            return Json(gallery.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Customers
        public ActionResult Index()
        {
            var controllerName = Resources.Language.Customers;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            var currentUser = User.Identity.GetUserId();
            var customers = userRepository.GetAllCustomers(currentUser);

            CurrentSession.SortBy = null;

            return View(customers);
        }

        // GET: Customers
        [HttpGet]
        [ActionName("IndexPartial")]
        public ActionResult _Index()
        {
            var controllerName = Resources.Language.Customers;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            var currentUser = User.Identity.GetUserId();
            var customers = userRepository.GetAllCustomers(currentUser);

            CurrentSession.SortBy = null;

            return PartialView("_Index", customers);
        }

        [HttpGet]
        [Route("Founder/Customers/Profile")]
        [ActionName("Profile")]
        public ActionResult ProfileIndex(string email)
        {
            var userStore = new UserStore<UserModel>(db);
            var UserManager = new UserManager<UserModel>(userStore);
            var userForProfile = db.Users.FirstOrDefault(w => w.Email == email);
            var role = UserManager.GetRoles(userForProfile.Id).FirstOrDefault();
            var userRole = db.Roles.SingleOrDefault(m => m.Name == role);
            var user = db.Users.FirstOrDefault(m => m.Roles.Any(r => r.RoleId == userRole.Id) && (m.Email == email));

            if (user != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user2 = db.Users.FirstOrDefault(m => m.Email == User.Identity.Name);
                    if (user.Id == user2.Id)
                    {
                        return RedirectToAction("Index", "Customers");
                    }
                }

                var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
                var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
                var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day : user.CreationDate.Day.ToString();
                var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month : user.CreationDate.Month.ToString();
                var date = day + "-" + month + "-" + user.CreationDate.Year;

                var controller = "/Admin/Customers/Index";
                var controllerName = Resources.Language.Customers;
                var controllerPartial = "/Admin/Customers/IndexPartial";
                var actionName = Resources.Language.ProfileTitle;

                TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                               (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
                TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
                TempData["profileStatus"] = user.ProfileMainStatus ?? true;

                return View("Profile");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet]
        [ActionName("IndexProfile")]
        public ActionResult _IndexProfile(string email)
        {
            var userStore = new UserStore<UserModel>(db);
            var UserManager = new UserManager<UserModel>(userStore);
            var userForProfile = db.Users.FirstOrDefault(w => w.Email == email);
            var role = UserManager.GetRoles(userForProfile.Id).FirstOrDefault();
            var userRole = db.Roles.SingleOrDefault(m => m.Name == role);
            var user = db.Users.FirstOrDefault(m => m.Roles.Any(r => r.RoleId == userRole.Id) && (m.Email == email));

            if (User.Identity.IsAuthenticated)
            {
                var user2 = db.Users.FirstOrDefault(m => m.Email == User.Identity.Name);
                if (user.Id == user2.Id)
                {
                    return RedirectToAction("Index", "Customers");
                }
            }

            var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
            var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
            var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day : user.CreationDate.Day.ToString();
            var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month : user.CreationDate.Month.ToString();
            var date = day + "-" + month + "-" + user.CreationDate.Year;

            var controller = "/Admin/Customers/Index";
            var controllerName = Resources.Language.Customers;
            var controllerPartial = "/Admin/Customers/IndexPartial";
            var actionName = Resources.Language.ProfileTitle;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            TempData["profileEmail"] = user.Email;
            TempData["profileFirstName"] = user.FirstName;
            TempData["profileLastName"] = user.LastName;
            TempData["profileCreationDate"] = date;
            TempData["profileGender"] = gender;
            TempData["profilePictureCount"] = profilePictureCount;
            TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                           (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
            TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
            TempData["profileStatus"] = user.ProfileMainStatus ?? true;

            return PartialView("_IndexProfile");
        }

        [HttpPost]
        [ActionName("PhotosProfile")]
        public ActionResult _PhotosProfile(string email)
        {

            if (email != null)
            {
                var userStore = new UserStore<UserModel>(db);
                var UserManager = new UserManager<UserModel>(userStore);
                var userForProfile = db.Users.FirstOrDefault(w => w.Email == email);
                var role = UserManager.GetRoles(userForProfile.Id).FirstOrDefault();
                var userRole = db.Roles.SingleOrDefault(m => m.Name == role);
                var user = db.Users.FirstOrDefault(m => m.Roles.Any(r => r.RoleId == userRole.Id) && (m.Email == email));

                if (User.Identity.IsAuthenticated)
                {
                    var user2 = db.Users.FirstOrDefault(m => m.Email == User.Identity.Name);
                    if (user.Id == user2.Id)
                    {
                        return RedirectToAction("Index", "Customers");
                    }
                }

                var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
                var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
                var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day : user.CreationDate.Day.ToString();
                var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month : user.CreationDate.Month.ToString();
                var date = day + "-" + month + "-" + user.CreationDate.Year;

                var controller = "/Admin/Customers/Index";
                var controllerName = Resources.Language.Customers;
                var controllerPartial = "/Admin/Customers/IndexPartial";
                var actionName = Resources.Language.ProfileTitle;

                TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                               (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
                TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
                TempData["profileStatus"] = user.ProfilePhotosStatus ?? true;

                return PartialView("_PhotosProfile");
            }
            else
            {
                return RedirectToAction("Index", "Customers");
            }
        }

        private static bool IsValidEmailAddress(string emailAddress)
        {
            return new System.ComponentModel.DataAnnotations
                                .EmailAddressAttribute()
                                .IsValid(emailAddress);
        }
        
    }
}
