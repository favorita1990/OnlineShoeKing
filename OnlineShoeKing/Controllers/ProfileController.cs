

namespace OnlineShoeKing.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Context;
    using ImageResizer;
    using Microsoft.AspNet.Identity;

    using Models;

    using Repository;

    public class ProfileController : Controller
    {
        private readonly DBContext db = new DBContext();
        private readonly UserRepository userRepository = new UserRepository();
        private readonly BreadCrumbRepository breadCrumbRepository = new BreadCrumbRepository();

        public JsonResult GetPhotosAlbum()
        {
            var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
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

        [HttpPost]
        public JsonResult ChangeProfileMainStatus(int radioVal)
        {
            var userId = User.Identity.GetUserId();

            userRepository.ChangeProfileMainStatus(userId, radioVal);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeProfilePhotosStatus(int radioVal)
        {
            var userId = User.Identity.GetUserId();

            userRepository.ChangeProfilePhotosStatus(userId, radioVal);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        // GET: Profile
        public ActionResult Index()
        {
            if (User != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
                    var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
                    var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
                    var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day : user.CreationDate.Day.ToString();
                    var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month : user.CreationDate.Month.ToString();
                    var date = day + "-" + month + "-" + user.CreationDate.Year;

                    var controllerName = Resources.Language.ProfileTitle;

                    TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

                    TempData["profileEmail"] = user.Email;
                    TempData["profileFirstName"] = user.FirstName;
                    TempData["profileLastName"] = user.LastName;
                    TempData["profileCreationDate"] = date;
                    TempData["profileGender"] = gender;
                    TempData["profilePictureCount"] = profilePictureCount;
                    TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                                   (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
                    TempData["profilePhotoCheck"] = user.ImageUrl;
                    TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
                    TempData["profileCoverCheck"] = user.CoverUrl;
                    TempData["profileStatus"] = user.ProfileMainStatus ?? true;

                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Profile
        [HttpGet, ActionName("IndexPartial")]
        public ActionResult _Index()
        {
            var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
            var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
            var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
            var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day : user.CreationDate.Day.ToString();
            var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month : user.CreationDate.Month.ToString();
            var date = day + "-" + month + "-" + user.CreationDate.Year;

            var controllerName = Resources.Language.ProfileTitle;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            TempData["profileEmail"] = user.Email;
            TempData["profileFirstName"] = user.FirstName;
            TempData["profileLastName"] = user.LastName;
            TempData["profileCreationDate"] = date;
            TempData["profileGender"] = gender;
            TempData["profilePictureCount"] = profilePictureCount;
            TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                           (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
            TempData["profilePhotoCheck"] = user.ImageUrl;
            TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
            TempData["profileCoverCheck"] = user.CoverUrl;
            TempData["profileStatus"] = user.ProfileMainStatus ?? true;

            return PartialView("_Index");
        }

        // GET: Photos
        public ActionResult Photos()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
                var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
                var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
                var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day : user.CreationDate.Day.ToString();
                var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month : user.CreationDate.Month.ToString();
                var date = day + "-" + month + "-" + user.CreationDate.Year;

                var controllerName = Resources.Language.ProfileTitle;

                TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                               (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
                TempData["profilePhotoCheck"] = user.ImageUrl;
                TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
                TempData["profileCoverCheck"] = user.CoverUrl;
                TempData["profileStatus"] = user.ProfilePhotosStatus ?? true;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // GET: Photos
        [HttpGet, ActionName("PhotosPartial")]
        public ActionResult _Photos()
        {
            var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
            var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
            var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
            var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day : user.CreationDate.Day.ToString();
            var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month : user.CreationDate.Month.ToString();
            var date = day + "-" + month + "-" + user.CreationDate.Year;

            var controllerName = Resources.Language.ProfileTitle;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            TempData["profileEmail"] = user.Email;
            TempData["profileFirstName"] = user.FirstName;
            TempData["profileLastName"] = user.LastName;
            TempData["profileCreationDate"] = date;
            TempData["profileGender"] = gender;
            TempData["profilePictureCount"] = profilePictureCount;
            TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                           (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
            TempData["profilePhotoCheck"] = user.ImageUrl;
            TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
            TempData["profileCoverCheck"] = user.CoverUrl;
            TempData["profileStatus"] = user.ProfilePhotosStatus ?? true;

            return PartialView("_Photos");
        }

        //Return view index --> Profile photo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateIndex(HttpPostedFileBase file)
        {
            var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
            var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
            var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
            var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day : user.CreationDate.Day.ToString();
            var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month : user.CreationDate.Month.ToString();
            var date = day + "-" + month + "-" + user.CreationDate.Year;

            if (file == null)
            {
                var fullPath = Server.MapPath(Resources.Pictures.FolderUsersImages + user.ImageUrl);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }

                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman;
                TempData["profilePhotoCheck"] = null;
                TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
                TempData["profileCoverCheck"] = user.CoverUrl;

                user.ImageUrl = null;
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }

            if (ModelState.IsValid)
            {
                var fullPath = Server.MapPath(Resources.Pictures.FolderUsersImages + user.ImageUrl);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }

                var time = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filename = time + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filename);
                file.SaveAs(filePath);


                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(file.FileName);
                fullPath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filenameT);
                file.SaveAs(fullPath);


                using (Image myImage = Image.FromStream(file.InputStream))
                {
                    Bitmap imageMedium = ResizeImage(myImage, 399, 150);

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


                //Image img = Image.FromFile(filePath);
                //int width = img.Width;
                //int height = img.Height;

                //img.Dispose();

                //ResizeSettings resizeSetting = new ResizeSettings
                //{
                //    Width = 832,
                //    Height = 500,
                //    Format = "jpg"
                //};

                //ImageBuilder.Current.Build(Resources.Pictures.FolderUsersImagesNoSlash + filename, Resources.Pictures.FolderUsersImagesNoSlash + filename, resizeSetting);

                //if (width > height)
                //{

                //    using (Image image = Image.FromFile(filePath))
                //    {

                //        //rotate the picture by 90 degrees and re-save the picture as a Jpeg
                //        image.RotateFlip(RotateFlipType.Rotate90FlipNone);

                //        // Save the image to the new_path
                //        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                //    }

                //}

                user.ImageUrl = filename;
                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = Resources.Pictures.FolderUsersImages + user.ImageUrl;
                TempData["profilePhotoCheck"] = filename;
                TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
                TempData["profileCoverCheck"] = user.CoverUrl;
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }

            return RedirectToAction("Index", "Profile");
        }

        //Return view photos --> profile photo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePhotos(HttpPostedFileBase file)
        {
            var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
            var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
            var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
            var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day.ToString() : user.CreationDate.Day.ToString();
            var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month.ToString() : user.CreationDate.Month.ToString();
            var date = day + "-" + month + "-" + user.CreationDate.Year.ToString();


            if (file == null)
            {
                var fullPath = Server.MapPath(Resources.Pictures.FolderUsersImages + user.ImageUrl);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }

                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman;
                TempData["profilePhotoCheck"] = null;
                TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
                TempData["profileCoverCheck"] = user.CoverUrl;
                user.ImageUrl = null;
                db.SaveChanges();
                return RedirectToAction("Photos", "Profile");
            }

            if (ModelState.IsValid)
            {
                var fullPath = Server.MapPath(Resources.Pictures.FolderUsersImages + user.ImageUrl);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }

                var time = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                var filename = time + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filename);
                file.SaveAs(filePath);

                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(file.FileName);
                fullPath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filenameT);
                file.SaveAs(fullPath);


                using (Image myImage = Image.FromStream(file.InputStream))
                {
                    Bitmap imageMedium = ResizeImage(myImage, 399, 150);

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

                user.ImageUrl = filename;
                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = Resources.Pictures.FolderUsersImages + user.ImageUrl;
                TempData["profilePhotoCheck"] = user.ImageUrl;
                TempData["profileCover"] = Resources.Pictures.FolderUsersImages + user.CoverUrl;
                TempData["profileCoverCheck"] = user.CoverUrl;
                db.SaveChanges();
                return RedirectToAction("Photos", "Profile");
            }
            return RedirectToAction("Photos", "Profile");
        }

        //Return view index --> Cover
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCoverIndex(HttpPostedFileBase file)
        {
            var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
            var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
            var gender = user.Gender == "0" ? "Man" : "Woman";
            var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day.ToString() : user.CreationDate.Day.ToString();
            var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month.ToString() : user.CreationDate.Month.ToString();
            var date = day + "-" + month + "-" + user.CreationDate.Year.ToString();

            if (file == null)
            {
                var fullPath = Server.MapPath(Resources.Pictures.FolderUsersImages + user.CoverUrl);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }
                user.CoverUrl = null;
                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                               (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
                TempData["profilePhotoCheck"] = user.ImageUrl;
                TempData["profileCover"] = Resources.Pictures.ProfileCover;
                TempData["profileCoverCheck"] = null;
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }

            if (ModelState.IsValid)
            {
                var fullPath = Server.MapPath(Resources.Pictures.FolderUsersImages + user.CoverUrl);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }

                var time = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                var filename = time + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filename);
                file.SaveAs(filePath);

                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(file.FileName);
                fullPath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filenameT);
                file.SaveAs(fullPath);


                using (Image myImage = Image.FromStream(file.InputStream))
                {
                    Bitmap imageMedium = ResizeImage(myImage, 832, 350);

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

                user.CoverUrl = filename;
                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                               (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
                TempData["profilePhotoCheck"] = user.ImageUrl;
                TempData["profileCover"] = Resources.Pictures.FolderUsersImages + user.CoverUrl;
                TempData["profileCoverCheck"] = user.CoverUrl;
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }

            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCoverPhotos(HttpPostedFileBase file)
        {
            var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
            var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
            var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
            var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day : user.CreationDate.Day.ToString();
            var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month : user.CreationDate.Month.ToString();
            var date = day + "-" + month + "-" + user.CreationDate.Year;

            if (file == null)
            {
                var fullPath = Server.MapPath(Resources.Pictures.FolderUsersImages + user.CoverUrl);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }
                user.CoverUrl = null;
                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                               (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
                TempData["profilePhotoCheck"] = user.ImageUrl;
                TempData["profileCover"] = Resources.Pictures.ProfileCover;
                TempData["profileCoverCheck"] = null;
                db.SaveChanges();
                return RedirectToAction("Photos", "Profile");
            }

            if (ModelState.IsValid)
            {
                var fullPath = Server.MapPath(Resources.Pictures.FolderUsersImages + user.CoverUrl);
                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath);
                }

                var time = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                var filename = time + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filename);
                file.SaveAs(filePath);

                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(file.FileName);
                fullPath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filenameT);
                file.SaveAs(fullPath);

                using (Image myImage = Image.FromStream(file.InputStream))
                {
                    Bitmap imageMedium = ResizeImage(myImage, 700, 350);

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

                user.CoverUrl = filename;
                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                               (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
                TempData["profilePhotoCheck"] = user.ImageUrl;
                TempData["profileCover"] = Resources.Pictures.FolderUsersImages + user.CoverUrl;
                TempData["profileCoverCheck"] = user.CoverUrl;
                db.SaveChanges();
                return RedirectToAction("Photos", "Profile");
            }

            return RedirectToAction("Photos", "Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PhotoAlbum(IEnumerable<HttpPostedFileBase> files)
        {
            var user = db.Users.FirstOrDefault(w => w.Email == User.Identity.Name);
            var profilePictureCount = db.Galleries.Count(w => w.UserId == user.Id);
            var gender = user.Gender == "0" ? Resources.Language.ManTitle : Resources.Language.WomanTitle;
            var day = user.CreationDate.Day < 10 ? "0" + user.CreationDate.Day.ToString() : user.CreationDate.Day.ToString();
            var month = user.CreationDate.Month < 10 ? "0" + user.CreationDate.Month.ToString() : user.CreationDate.Month.ToString();
            var date = day + "-" + month + "-" + user.CreationDate.Year.ToString();

            if (files == null)
            {
                TempData["profileEmail"] = user.Email;
                TempData["profileFirstName"] = user.FirstName;
                TempData["profileLastName"] = user.LastName;
                TempData["profileCreationDate"] = date;
                TempData["profileGender"] = gender;
                TempData["profilePictureCount"] = profilePictureCount;
                TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                               (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
                TempData["profilePhotoCheck"] = user.ImageUrl;
                TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
                TempData["profileCoverCheck"] = user.CoverUrl;
                return View("Index");
            }

            foreach (var file in files)
            {
                if (ModelState.IsValid)
                {
                    Gallery gallery = new Gallery();

                    string time = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                    DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    var filename = time + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filename);
                    file.SaveAs(filePath);

                    var timeT = DateTime.Now.Year + DateTime.Now.Month +
                    DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                    var filenameT = timeT + "_T" + Path.GetFileName(file.FileName);
                    var fullPath = Path.Combine(Server.MapPath(Resources.Pictures.FolderUsersImages), filenameT);
                    file.SaveAs(fullPath);


                    using (Image myImage = Image.FromStream(file.InputStream))
                    {
                        Bitmap imageMedium = ResizeImage(myImage, 800, 350);

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
                    gallery.Time = DateTime.Now;
                    gallery.ImageUrl = filename;
                    gallery.UserId = user.Id;
                    db.Galleries.Add(gallery);
                    db.SaveChanges();
                }
            }

            TempData["profileEmail"] = user.Email;
            TempData["profileFirstName"] = user.FirstName;
            TempData["profileLastName"] = user.LastName;
            TempData["profileCreationDate"] = date;
            TempData["profileGender"] = gender;
            TempData["profilePictureCount"] = profilePictureCount;
            TempData["profilePhoto"] = user.ImageUrl != null ? Resources.Pictures.FolderUsersImages + user.ImageUrl :
                                           (user.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);
            TempData["profilePhotoCheck"] = user.ImageUrl;
            TempData["profileCover"] = user.CoverUrl != null ? Resources.Pictures.FolderUsersImages + user.CoverUrl : Resources.Pictures.ProfileCover;
            TempData["profileCoverCheck"] = user.CoverUrl;

            return RedirectToAction("Photos", "Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePhoto(int[] deleteInputs)
        {

            if (deleteInputs != null && deleteInputs.Length > 0)
            {
                for (int i = 0; i < deleteInputs.Length; i++)
                {
                    Gallery image = db.Galleries.Find(deleteInputs[i]);
                    var fullPath = Server.MapPath(Resources.Pictures.FolderUsersImages + image.ImageUrl);
                    if (System.IO.File.Exists(fullPath))
                    {

                        System.IO.File.Delete(fullPath);
                    }
                    db.Galleries.Remove(image);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("PhotosPartial");
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
