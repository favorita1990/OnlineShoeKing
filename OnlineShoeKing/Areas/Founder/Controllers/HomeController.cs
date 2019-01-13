

namespace OnlineShoeKing.Areas.Founder.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Context;
    using Models;
    using Repository;
    using System.Drawing.Imaging;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    [Authorize(Roles = "Founder")]
    public class HomeController : Controller
    {
        private readonly DBContext db = new DBContext();
        private readonly ProductRepository productRepository = new ProductRepository();
        private readonly CategoryRepository categoryRepository = new CategoryRepository();
        private readonly BreadCrumbRepository breadCrumbRepository = new BreadCrumbRepository();
        private readonly string profilePic = Resources.Pictures.AboutPic;
        private readonly string profilePicWoman = Resources.Pictures.ProfileWoman;
        private readonly string directoryToProfilePicture = Resources.Pictures.FolderUsersImages;
        private readonly string directoryOfProductIco = Resources.Pictures.PicProducts;
        private readonly string getCustomersPicPath = Resources.Pictures.PicPeople;

        [HttpPost]
        public JsonResult EditHomePagePicture()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files["homeChangeImg"];

                if (Path.GetExtension(file.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(file.FileName).ToLower() != ".png"
                    && Path.GetExtension(file.FileName).ToLower() != ".gif"
                    && Path.GetExtension(file.FileName).ToLower() != ".jpeg")
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var format = Path.GetExtension(file.FileName);
                var fileName = Utilities.GenerateRandomString() + format;
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.HomePageImage), fileName);
                file.SaveAs(filePath);

                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(file.FileName);
                var fullPath = Path.Combine(Server.MapPath(Resources.Pictures.HomePageImage), filenameT);
                file.SaveAs(fullPath);

                using (Image myImage = Image.FromStream(file.InputStream))
                {
                    Bitmap imageMedium = ResizeImage(myImage, 1600, 800);

                    SaveJpeg(filePath, imageMedium, 90);

                    imageMedium.Dispose();
                }

                Image img = Image.FromFile(fullPath);
                if (img.PropertyIdList.Contains(0x0112))
                {
                    PropertyItem propOrientation = img.GetPropertyItem(0x0112);
                    short orientation = BitConverter.ToInt16(propOrientation.Value, 0);
                    if (orientation == 6)
                    {
                        Image imgFull = Image.FromFile(filePath);

                        imgFull.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        imgFull.Save(filePath, ImageFormat.Jpeg);
                        imgFull.Dispose();
                    }
                    else if (orientation == 8)
                    {
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        img.Save(filePath, ImageFormat.Jpeg);
                    }
                    else
                    {
                        // Do nothing
                    }
                }

                img.Dispose();

                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }
                var fileFullPath = $"/images/home-page/{fileName}";
                var homePage = this.db.HomePage.FirstOrDefault();
                fullPath = Server.MapPath(Resources.Pictures.HomePageImage + homePage.ImageUrl);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }
                homePage.ImageUrl = fileName;
                db.SaveChanges();

                return Json(fileFullPath, JsonRequestBehavior.AllowGet);

            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult DeletePictureFirst()
        {
            var dateTimeNow = DateTime.Now;
            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.db.Users.Find(currentUserId);
            var about = this.db.About.FirstOrDefault();
            var fullPath = Server.MapPath(Resources.Pictures.AboutFolder + about.FirstImage);
            if (System.IO.File.Exists(fullPath))
            {

                System.IO.File.Delete(fullPath);
            }
            about.FirstImage = null;
            var fullName = $"{currentUser.FirstName} {currentUser.LastName}";
            about.UpdatedFirstImageBy = currentUserId;
            about.UpdatedFirstImageAt = dateTimeNow;
            db.SaveChanges();
            List<Tuple<string, string, string>> images = new List<Tuple<string, string, string>>()
                                                             {
                                                                 new Tuple<string, string, string>("", fullName, dateTimeNow.ToString("dd.MM.yyyy HH:mm"))
                                                             };

            return Json(images, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePictureSecond()
        {
            var dateTimeNow = DateTime.Now;
            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.db.Users.Find(currentUserId);
            var about = this.db.About.FirstOrDefault();
            var fullPath = Server.MapPath(Resources.Pictures.AboutFolder + about.SecondImage);
            if (System.IO.File.Exists(fullPath))
            {

                System.IO.File.Delete(fullPath);
            }
            about.SecondImage = null;
            var fullName = $"{currentUser.FirstName} {currentUser.LastName}";
            about.UpdatedSecondImageBy = currentUserId;
            about.UpdatedSecondImageAt = dateTimeNow;
            db.SaveChanges();
            List<Tuple<string, string, string>> images = new List<Tuple<string, string, string>>()
                                                             {
                                                                 new Tuple<string, string, string>("", fullName, dateTimeNow.ToString("dd.MM.yyyy HH:mm"))
                                                             };

            return Json(images, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("IndexPartial")]
        public ActionResult _Index()
        {
            return PartialView("_Index");
        }
        
        public ActionResult About()
        {
            var controller = "/Founder/Home/Index";
            var controllerName = "Home";
            var controllerPartial = "/Founder/Home/IndexPartial";
            var actionName = Resources.Language.AboutUs;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            var about = this.db.About.FirstOrDefault();

            var updatedFirstImageBy = "";
            if (about.UpdatedFirstImageBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedFirstImageBy);
                updatedFirstImageBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedFirstTextBy = "";
            if (about.UpdatedFirstTextBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedFirstTextBy);
                updatedFirstTextBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedSecondImageBy = "";
            if (about.UpdatedSecondImageBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedSecondImageBy);
                updatedSecondImageBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedSecondTextBy = "";
            if (about.UpdatedSecondTextBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedSecondTextBy);
                updatedSecondTextBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            TempData["firstPic"] = about.FirstImage != null ? Resources.Pictures.AboutFolder + about.FirstImage : "";
            TempData["firstPicUpdatedby"] = updatedFirstImageBy;
            TempData["firstPicUpdatedAt"] = about.UpdatedFirstImageAt.HasValue ? about.UpdatedFirstImageAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["textFirstHeader"] = about.FirstTextHeader;
            TempData["textFirst"] = about.FirstText;
            TempData["updatedFirstTextBy"] = updatedFirstTextBy;
            TempData["updatedFirstTextAt"] = about.UpdatedFirstTextAt.HasValue ? about.UpdatedFirstTextAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["secondPic"] = about.SecondImage != null ? Resources.Pictures.AboutFolder + about.SecondImage : "";
            TempData["secondPicUpdatedby"] = updatedSecondImageBy;
            TempData["secondPicUpdatedAt"] = about.UpdatedSecondImageAt.HasValue ? about.UpdatedSecondImageAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["textSecondHeader"] = about.SecondTextHeader;
            TempData["textSecond"] = about.SecondText;
            TempData["textSecondUpdatedBy"] = updatedSecondTextBy;
            TempData["textSecondUpdatedAt"] = about.UpdatedSecondTextAt.HasValue ? about.UpdatedSecondTextAt.Value.ToString("dd.MM.yy HH:mm") : "";

            return View();
        }

        [HttpGet]
        [ActionName("AboutPartial")]
        public ActionResult _About()
        {
            var controller = "/Founder/Home/Index";
            var controllerName = "Home";
            var controllerPartial = "/Founder/Home/IndexPartial";
            var actionName = Resources.Language.AboutUs;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            var about = this.db.About.FirstOrDefault();

            var updatedFirstImageBy = "";
            if (about.UpdatedFirstImageBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedFirstImageBy);
                updatedFirstImageBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedFirstTextBy = "";
            if (about.UpdatedFirstTextBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedFirstTextBy);
                updatedFirstTextBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedSecondImageBy = "";
            if (about.UpdatedSecondImageBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedSecondImageBy);
                updatedSecondImageBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedSecondTextBy = "";
            if (about.UpdatedSecondTextBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedSecondTextBy);
                updatedSecondTextBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            TempData["firstPic"] = about.FirstImage != null ? Resources.Pictures.AboutFolder + about.FirstImage : "";
            TempData["firstPicUpdatedby"] = updatedFirstImageBy;
            TempData["firstPicUpdatedAt"] = about.UpdatedFirstImageAt.HasValue ? about.UpdatedFirstImageAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["textFirstHeader"] = about.FirstTextHeader;
            TempData["textFirst"] = about.FirstText;
            TempData["updatedFirstTextBy"] = updatedFirstTextBy;
            TempData["updatedFirstTextAt"] = about.UpdatedFirstTextAt.HasValue ? about.UpdatedFirstTextAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["secondPic"] = about.SecondImage != null ? Resources.Pictures.AboutFolder + about.SecondImage : "";
            TempData["secondPicUpdatedby"] = updatedSecondImageBy;
            TempData["secondPicUpdatedAt"] = about.UpdatedSecondImageAt.HasValue ? about.UpdatedSecondImageAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["textSecondHeader"] = about.SecondTextHeader;
            TempData["textSecond"] = about.SecondText;
            TempData["textSecondUpdatedBy"] = updatedSecondTextBy;
            TempData["textSecondUpdatedAt"] = about.UpdatedSecondTextAt.HasValue ? about.UpdatedSecondTextAt.Value.ToString("dd.MM.yy HH:mm") : "";

            return PartialView("_About");
        }

        [HttpPost]
        public JsonResult EditPictureFirst()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files["pictureFirst"];

                if (Path.GetExtension(file.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(file.FileName).ToLower() != ".png"
                    && Path.GetExtension(file.FileName).ToLower() != ".gif"
                    && Path.GetExtension(file.FileName).ToLower() != ".jpeg")
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var format = Path.GetExtension(file.FileName);
                var fileName = Utilities.GenerateRandomString() + format;
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.AboutFolder), fileName);
                file.SaveAs(filePath);

                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(file.FileName);
                var fullPath = Path.Combine(Server.MapPath(Resources.Pictures.AboutFolder), filenameT);
                file.SaveAs(fullPath);

                using (Image myImage = Image.FromStream(file.InputStream))
                {
                    Bitmap imageMedium = ResizeImage(myImage, 1200, 800);

                    SaveJpeg(filePath, imageMedium, 90);

                    imageMedium.Dispose();
                }

                Image img = Image.FromFile(fullPath);
                if (img.PropertyIdList.Contains(0x0112))
                {
                    PropertyItem propOrientation = img.GetPropertyItem(0x0112);
                    short orientation = BitConverter.ToInt16(propOrientation.Value, 0);
                    if (orientation == 6)
                    {
                        Image imgFull = Image.FromFile(filePath);

                        imgFull.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        imgFull.Save(filePath, ImageFormat.Jpeg);
                        imgFull.Dispose();
                    }
                    else if (orientation == 8)
                    {
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        img.Save(filePath, ImageFormat.Jpeg);
                    }
                    else
                    {
                        // Do nothing
                    }
                }

                img.Dispose();

                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }
                var fileFullPath = $"\\images\\about\\{fileName}";
                var dateTimeNow = DateTime.Now;
                var currentUserId = User.Identity.GetUserId();
                var currentUser = this.db.Users.Find(currentUserId);
                var fullName = $"{currentUser.FirstName} {currentUser.LastName}";
                var about = this.db.About.FirstOrDefault();
                fullPath = Server.MapPath(Resources.Pictures.AboutFolder + about.FirstImage);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }
                about.FirstImage = fileName;
                about.UpdatedFirstImageBy = currentUserId;
                about.UpdatedFirstImageAt = dateTimeNow;
                db.SaveChanges();
                List<Tuple<string, string, string>> images = new List<Tuple<string, string, string>>()
                                                                 {
                                                                     new Tuple<string, string, string>(fileFullPath, fullName, dateTimeNow.ToString("dd.MM.yyyy HH:mm"))
                                                                 };

                return Json(images, JsonRequestBehavior.AllowGet);

            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult EditWhoFirst(string textHeader, string textBody)
        {
            var controller = "/Founder/Home/Index";
            var controllerName = "Home";
            var controllerPartial = "/Founder/Home/IndexPartial";
            var actionName = Resources.Language.AboutUs;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            var dateTimeNow = DateTime.Now;
            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.db.Users.Find(currentUserId);
            var about = this.db.About.FirstOrDefault();
            about.UpdatedFirstTextBy = currentUser.Id;
            about.UpdatedFirstTextAt = dateTimeNow;
            about.FirstTextHeader = textHeader;
            about.FirstText = textBody;
            this.db.SaveChanges();

            var updatedFirstImageBy = "";
            if (about.UpdatedFirstImageBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedFirstImageBy);
                updatedFirstImageBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedFirstTextBy = "";
            if (about.UpdatedFirstTextBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedFirstTextBy);
                updatedFirstTextBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedSecondImageBy = "";
            if (about.UpdatedSecondImageBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedSecondImageBy);
                updatedSecondImageBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedSecondTextBy = "";
            if (about.UpdatedSecondTextBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedSecondTextBy);
                updatedSecondTextBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            TempData["firstPic"] = about.FirstImage != null ? Resources.Pictures.AboutFolder + about.FirstImage : "";
            TempData["firstPicUpdatedby"] = updatedFirstImageBy;
            TempData["firstPicUpdatedAt"] = about.UpdatedFirstImageAt.HasValue ? about.UpdatedFirstImageAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["textFirstHeader"] = about.FirstTextHeader;
            TempData["textFirst"] = about.FirstText;
            TempData["updatedFirstTextBy"] = updatedFirstTextBy;
            TempData["updatedFirstTextAt"] = about.UpdatedFirstTextAt.HasValue ? about.UpdatedFirstTextAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["secondPic"] = about.SecondImage != null ? Resources.Pictures.AboutFolder + about.SecondImage : "";
            TempData["secondPicUpdatedby"] = updatedSecondImageBy;
            TempData["secondPicUpdatedAt"] = about.UpdatedSecondImageAt.HasValue ? about.UpdatedSecondImageAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["textSecondHeader"] = about.SecondTextHeader;
            TempData["textSecond"] = about.SecondText;
            TempData["textSecondUpdatedBy"] = updatedSecondTextBy;
            TempData["textSecondUpdatedAt"] = about.UpdatedSecondTextAt.HasValue ? about.UpdatedSecondTextAt.Value.ToString("dd.MM.yy HH:mm") : "";

            return PartialView("_About");
        }

        [HttpPost]
        public JsonResult EditPictureSecond()
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["pictureSecond"];

                if (Path.GetExtension(file.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(file.FileName).ToLower() != ".png"
                    && Path.GetExtension(file.FileName).ToLower() != ".gif"
                    && Path.GetExtension(file.FileName).ToLower() != ".jpeg")
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var format = Path.GetExtension(file.FileName);
                var fileName = Utilities.GenerateRandomString() + format;
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.AboutFolder), fileName);
                file.SaveAs(filePath);

                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(file.FileName);
                var fullPath = Path.Combine(Server.MapPath(Resources.Pictures.AboutFolder), filenameT);
                file.SaveAs(fullPath);

                using (Image myImage = Image.FromStream(file.InputStream))
                {
                    Bitmap imageMedium = ResizeImage(myImage, 1200, 800);

                    SaveJpeg(filePath, imageMedium, 90);

                    imageMedium.Dispose();
                }

                Image img = Image.FromFile(fullPath);
                if (img.PropertyIdList.Contains(0x0112))
                {
                    PropertyItem propOrientation = img.GetPropertyItem(0x0112);
                    short orientation = BitConverter.ToInt16(propOrientation.Value, 0);
                    if (orientation == 6)
                    {
                        Image imgFull = Image.FromFile(filePath);

                        imgFull.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        imgFull.Save(filePath, ImageFormat.Jpeg);
                        imgFull.Dispose();
                    }
                    else if (orientation == 8)
                    {
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        img.Save(filePath, ImageFormat.Jpeg);
                    }
                    else
                    {
                        // Do nothing
                    }
                }

                img.Dispose();

                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }
                var fileFullPath = $"\\images\\about\\{fileName}";
                var dateTimeNow = DateTime.Now;
                var currentUserId = User.Identity.GetUserId();
                var currentUser = this.db.Users.Find(currentUserId);
                var fullName = $"{currentUser.FirstName} {currentUser.LastName}";
                var about = this.db.About.FirstOrDefault();
                fullPath = Server.MapPath(Resources.Pictures.AboutFolder + about.SecondImage);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }
                about.SecondImage = fileName;
                about.UpdatedSecondImageBy = currentUserId;
                about.UpdatedSecondImageAt = dateTimeNow;
                db.SaveChanges();
                List<Tuple<string, string, string>> images = new List<Tuple<string, string, string>>()
                                                                 {
                                                                     new Tuple<string, string, string>(fileFullPath, fullName, dateTimeNow.ToString("dd.MM.yyyy HH:mm"))
                                                                 };

                return Json(images, JsonRequestBehavior.AllowGet);

            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult EditWhoSecond(string textHeader, string textBody)
        {
            var controller = "/Founder/Home/Index";
            var controllerName = "Home";
            var controllerPartial = "/Founder/Home/IndexPartial";
            var actionName = Resources.Language.AboutUs;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            var dateTimeNow = DateTime.Now;
            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.db.Users.Find(currentUserId);
            var about = this.db.About.FirstOrDefault();
            about.UpdatedSecondTextBy = currentUser.Id;
            about.UpdatedSecondTextAt = dateTimeNow;
            about.SecondTextHeader = textHeader;
            about.SecondText = textBody;
            this.db.SaveChanges();

            var updatedFirstImageBy = "";
            if (about.UpdatedFirstImageBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedFirstImageBy);
                updatedFirstImageBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedFirstTextBy = "";
            if (about.UpdatedFirstTextBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedFirstTextBy);
                updatedFirstTextBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedSecondImageBy = "";
            if (about.UpdatedSecondImageBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedSecondImageBy);
                updatedSecondImageBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            var updatedSecondTextBy = "";
            if (about.UpdatedSecondTextBy != null)
            {
                var updatedUser = this.db.Users.Find(about.UpdatedSecondTextBy);
                updatedSecondTextBy = $"{updatedUser.FirstName} {updatedUser.LastName}";
            }

            TempData["firstPic"] = about.FirstImage != null ? Resources.Pictures.AboutFolder + about.FirstImage : "";
            TempData["firstPicUpdatedby"] = updatedFirstImageBy;
            TempData["firstPicUpdatedAt"] = about.UpdatedFirstImageAt.HasValue ? about.UpdatedFirstImageAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["textFirstHeader"] = about.FirstTextHeader;
            TempData["textFirst"] = about.FirstText;
            TempData["updatedFirstTextBy"] = updatedFirstTextBy;
            TempData["updatedFirstTextAt"] = about.UpdatedFirstTextAt.HasValue ? about.UpdatedFirstTextAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["secondPic"] = about.SecondImage != null ? Resources.Pictures.AboutFolder + about.SecondImage : "";
            TempData["secondPicUpdatedby"] = updatedSecondImageBy;
            TempData["secondPicUpdatedAt"] = about.UpdatedSecondImageAt.HasValue ? about.UpdatedSecondImageAt.Value.ToString("dd.MM.yy HH:mm") : "";
            TempData["textSecondHeader"] = about.SecondTextHeader;
            TempData["textSecond"] = about.SecondText;
            TempData["textSecondUpdatedBy"] = updatedSecondTextBy;
            TempData["textSecondUpdatedAt"] = about.UpdatedSecondTextAt.HasValue ? about.UpdatedSecondTextAt.Value.ToString("dd.MM.yy HH:mm") : "";

            return PartialView("_About");
        }

        public static Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            double ratioWidth = (double)maxWidth / (double)image.Width;
            double ratioHeight = (double)maxHeight / (double)image.Height;

            double ratio = ratioWidth < ratioHeight ? ratioWidth : ratioHeight;

            int newWidth = Convert.ToInt32(image.Width * ratio);
            int newHeight = Convert.ToInt32(image.Height * ratio);

            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var destImage = new Bitmap(newWidth, newHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);


            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void SaveJpeg(string path, Bitmap img, int quality)
        {
            if (quality < 0 || quality > 100)
            {
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");
            }

            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            img.Save(path, jpegCodec, encoderParams);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }
    }
}