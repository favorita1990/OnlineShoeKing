

namespace OnlineShoeKing.Controllers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;

    using Context;
    using Models;

    using OnlineShoeKing.Repository;

    public class HomeController : Controller
    {
        private readonly DBContext db = new DBContext();
        private readonly ShoppingCart shoppingCartRepository = new ShoppingCart();
        private readonly ProductRepository productRepository = new ProductRepository();
        private readonly BreadCrumbRepository breadCrumbRepository = new BreadCrumbRepository();
        private readonly string profilePic = Resources.Pictures.ProfilePic;
        private readonly string profilePicWoman = Resources.Pictures.ProfileWoman;
        private readonly string directoryToProfilePicture = Resources.Pictures.FolderUsersImages;
        private string directoryOfProductIco = Resources.Pictures.PicProducts;
        private string getCustomersPicPath = Resources.Pictures.PicPeople;

        [HttpGet]
        public ActionResult GetHomePage()
        {
            var homePageDb = this.db.HomePage.FirstOrDefault();
            var homePage = new HomePageViewModel()
                                             {
                                                 Text = homePageDb.Text,
                                                 TextHeader = homePageDb.TextHeader,
                                                 ImageUrl = homePageDb.ImageUrl != "" ? Resources.Pictures.HomePageImage + homePageDb.ImageUrl : ""
                                             };

            return this.Json(homePage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFooter()
        {
            var categoryMen = db.Products?.Where(w => w.Category.Gender == false);
            var categoryWomen = db.Products?.Where(w => w.Category.Gender == true);
            var customersCount = db.Users.Count();

            var footer = new Footer()
            {
                MenCount = categoryMen.Count(),
                WomenCount = categoryWomen.Count(),
                CustomersCount = customersCount
            };

            return PartialView("_Footer", footer);
        }

        [HttpGet]
        public JsonResult GetCurrentUser()
        {
            var userDb = db.Users.FirstOrDefault(f => f.Email == User.Identity.Name);

            UserViewModel user = new UserViewModel()
             {
                 Email = userDb.Email,
                 FirstName = userDb.FirstName,
                 LastName = userDb.LastName,
                 ImageUrl = userDb.ImageUrl == null
                                ? (userDb.Gender == "0"
                                       ? profilePic
                                       : profilePicWoman)
                                : (directoryToProfilePicture + userDb.ImageUrl),
                 Gender = userDb.Gender
             };

            return Json(user, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNewArrivals()
        {
            var newArrivals = this.productRepository.GetNewArrivals();

            return this.Json(newArrivals, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMostBought()
        {
            var discounts = productRepository.GetMostBought();

            return Json(discounts, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult Language()
        {
            HttpCookie cookie = Request.Cookies["language"];
            string languageValue = "";
            if (cookie?.Value != null)
            {
                languageValue = cookie.Value == "en" ? "1" : "2";
            }
            else
            {
                languageValue = "1";
            }

            return Json(languageValue, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOrderMessagesCount()
        {
            var orderStatusId = db.OrderStatuses.FirstOrDefault(f => f.Name == "waiting")?.Id;
            var orderMessagesCount = db.Orders?.Where(w => w.OrderStatusId == orderStatusId && !w.Deleted).ToList().Count;

            return Json(orderMessagesCount, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddProductToCart(int productId, int sizeId)
        {

            List<CartViewModel> cartViewModel = shoppingCartRepository.AddToCart(productId, sizeId);

            return Json(cartViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveProductFromCart(int productId)
        {
            var total = shoppingCartRepository.RemoveFromCart(productId);

            return Json(total.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditHomeTextHeader(string textHeader)
        {
            var homePage = this.db.HomePage.FirstOrDefault();
            homePage.TextHeader = textHeader;
            this.db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditHomeText(string text)
        {
            var homePage = this.db.HomePage.FirstOrDefault();
            homePage.Text = text;
            this.db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public JsonResult IsValidEmail(string source)
        {
            bool isvalid = new EmailAddressAttribute().IsValid(source.Trim());

            if (!isvalid)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(isvalid.ToString().ToLower(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public static bool IsPhoneNumber(string number)
        {
            return !Regex.Match(number, @"^[0-9]\d{10}$").Success;
        }

        [HttpPost]
        public JsonResult SendEmail(string name, string email, string subject, string text)
        {

            if ((name == "") || (email == "") || (subject == "") || (text == ""))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            bool isvalid = new EmailAddressAttribute().IsValid(email.Trim());

            if (!isvalid)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var body = "<p>From Name: {0}</p><p>From Email: {1}</p><p>Text: {2}</p>";
            MailMessage mailMessage = new MailMessage();
            MailAddress fromAddress = new MailAddress(email);
            mailMessage.From = fromAddress;
            mailMessage.To.Add("wearenbu@gmail.com");
            mailMessage.Body = string.Format(body, name, email, text);
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;

            SmtpClient mailSender = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential("wearenbu@gmail.com", "wearenbu1")
            };

            mailSender.Send(mailMessage);


            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Checkout()
        {

            var shoppingCart = shoppingCartRepository.Checkout();

            switch (shoppingCart)
            {
                case -3:
                    return this.Json(null, JsonRequestBehavior.AllowGet);
                case -2:
                    return this.Json("login", JsonRequestBehavior.AllowGet);
                case -1:
                    return this.Json("problem", JsonRequestBehavior.AllowGet);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CartOrder()
        {
            var city = Request.Form["cartOrderCity"];
            var address = Request.Form["cartOrderAddress"];
            var postOfiice = Request.Form["cartOrderPostOfiice"];
            var phoneNumber = Request.Form["cartOrderPhoneNumber"].Trim();

            if (city == "")
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            if (address == "")
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            if (postOfiice == "")
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            if (!IsPhoneNumber(phoneNumber))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var shoppingCart = shoppingCartRepository.CartOrder(city, address, postOfiice, phoneNumber);

            switch (shoppingCart)
            {
                case -3:
                    return this.Json(null, JsonRequestBehavior.AllowGet);
                case -2:
                    return this.Json("login", JsonRequestBehavior.AllowGet);
                case -1:
                    return this.Json("problem", JsonRequestBehavior.AllowGet);
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckoutProblem()
        {

            var cartViewModel = shoppingCartRepository.CheckoutProblem();

            return Json(cartViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCartProducts()
        {
            var cartItems = shoppingCartRepository.GetCartProducts();

            return Json(cartItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Language(string lang)
        {
            lang = lang == "1" ? "en" : "bg";

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            HttpCookie cookie = new HttpCookie("language") { Value = lang };
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult DeleteHomePageImg()
        {
            var homePageDb = this.db.HomePage.FirstOrDefault();
            homePageDb.ImageUrl = "";
            this.db.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            if(User != null)
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    if (this.User.IsInRole("Founder"))
                    {
                        return this.RedirectToAction("Index", "Home", new { Area = "Founder" });
                    }

                    if (this.User.IsInRole("Admin"))
                    {
                        return this.RedirectToAction("Index", "Home", new { Area = "Admin" });
                    }
                }
            }

            return this.View();
        }

        [HttpGet]
        [ActionName("IndexPartial")]
        public ActionResult _Index()
        {
            return PartialView("_Index");
        }

        public ActionResult About()
        {
            if (User != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (User.IsInRole("Founder"))
                    {
                        return RedirectToAction("About", "Home", new { Area = "Founder" });
                    }

                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("About", "Home", new { Area = "Admin" });
                    }
                }
            }

            var controller = "/Home/Index";
            var controllerName = "Home";
            var controllerPartial = "/Home/IndexPartial";
            var actionName = Resources.Language.AboutUs;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            var about = this.db.About.FirstOrDefault();
            TempData["firstPic"] = about.FirstImage != null ? Resources.Pictures.AboutFolder + about.FirstImage : "";
            TempData["textFirstHeader"] = about.FirstTextHeader;
            TempData["textFirst"] = about.FirstText;
            TempData["secondPic"] = about.SecondImage != null ? Resources.Pictures.AboutFolder + about.SecondImage : "";
            TempData["textSecondHeader"] = about.SecondTextHeader;
            TempData["textSecond"] = about.SecondText;

            return View();
        }

        [HttpGet]
        [ActionName("AboutPartial")]
        public ActionResult _About()
        {
            var controller = "/Home/Index";
            var controllerName = "Home";
            var controllerPartial = "/Home/IndexPartial";
            var actionName = Resources.Language.AboutUs;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            var about = this.db.About.FirstOrDefault();
            TempData["firstPic"] = about.FirstImage != null ? Resources.Pictures.AboutFolder + about.FirstImage : "";
            TempData["textFirstHeader"] = about.FirstTextHeader;
            TempData["textFirst"] = about.FirstText;
            TempData["secondPic"] = about.SecondImage != null ? Resources.Pictures.AboutFolder + about.SecondImage : "";
            TempData["textSecondHeader"] = about.SecondTextHeader;
            TempData["textSecond"] = about.SecondText;

            return PartialView("_About");
        }

        public ActionResult Contact()
        {
            if (User != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (User.IsInRole("Founder"))
                    {
                        return RedirectToAction("Contact", "Home", new { Area = "Founder" });
                    }

                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Contact", "Home", new { Area = "Admin" });
                    }
                }
            }

            var controller = "/Home/Index";
            var controllerName = "Home";
            var controllerPartial = "/Home/IndexPartial";
            var actionName = Resources.Language.ContactUs;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            return View();
        }

        [HttpGet]
        [ActionName("ContactPartial")]
        public ActionResult _Contact()
        {
            var controller = "/Home/Index";
            var controllerName = "Home";
            var controllerPartial = "/Home/IndexPartial";
            var actionName = Resources.Language.ContactUs;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            return PartialView("_Contact");
        }
    }
}