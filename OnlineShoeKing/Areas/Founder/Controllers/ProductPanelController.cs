
namespace OnlineShoeKing.Areas.Founder.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;
    using Context;
    using Models;
    using Models.ViewModels;
    using Repository;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Drawing2D;

    [Authorize(Roles = "Founder")]
    public class ProductPanelController : Controller
    {
        private readonly DBContext db = new DBContext();
        private readonly ProductRepository productRepository = new ProductRepository();
        private readonly CategoryRepository categoryRepository = new CategoryRepository();


        [HttpPost]
        public ActionResult GetProductById(string productId)
        {
            ProductViewModel product = new ProductViewModel();

            product = productRepository.GetProductByIdGridEdit(int.Parse(productId));

            CurrentSession.ProductAddOrEditProductId = productId;

            return Json(product, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteProductById(string productId)
        {
            productRepository.DeleteProductById(int.Parse(productId));

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCategoryById(string categoryId)
        {
            CategoryViewModel category = new CategoryViewModel();

            category = categoryRepository.GetCategoryByIdGridEdit(int.Parse(categoryId));

            CurrentSession.CategoryAddOrEditCategoryId = categoryId;

            return Json(category, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteCategoryById(string categoryId)
        {
            categoryRepository.DeleteCategoryById(int.Parse(categoryId));

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAllCategories([DataSourceRequest]DataSourceRequest request)
        {

            IEnumerable<CategoryViewModel> allCategories = new List<CategoryViewModel>();

            allCategories = categoryRepository.GetAllCategories();

            return Json(allCategories.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetAllProducts([DataSourceRequest]DataSourceRequest request)
        {
            IEnumerable<ProductViewModel> allProducts = new List<ProductViewModel>();

            allProducts = productRepository.GetAllProductsGrid();

            return Json(allProducts.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetSizeOfProductPanel([DataSourceRequest]DataSourceRequest request, int productId)
        {
            IEnumerable<SizesOfProduct> sizeOfProducts = new List<SizesOfProduct>();

            sizeOfProducts = productRepository.GetSizeOfProductByProductId(productId);

            return Json(sizeOfProducts.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetPhotoOfProductPanel([DataSourceRequest]DataSourceRequest request, int productId)
        {
            IEnumerable<PhotoOfProductVM> photoOfProducts = new List<PhotoOfProductVM>();

            photoOfProducts = productRepository.GetPhotoOfProductByProductId(productId);

            return Json(photoOfProducts.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetProductsByCategoryId([DataSourceRequest]DataSourceRequest request, int categoryId)
        {
            IEnumerable<ProductViewModel> products = new List<ProductViewModel>();

            products = categoryRepository.GetProductByCategoryId(categoryId);

            return Json(products.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult CreateEditProduct(IEnumerable<HttpPostedFileBase> files)
        {
            string productId = CurrentSession.ProductAddOrEditProductId; 

            string productName = Request.Form["inputName"];
            string productBgName = Request.Form["inputBgName"];
            HttpPostedFileBase productMainImg = CurrentSession.ProductMainImage;
            IEnumerable<HttpPostedFileBase> productBodyImg = CurrentSession.ProductBodyImages;
            string productPriceReplaced = Request.Form["inputPrice"];
            productPriceReplaced = productPriceReplaced.Replace(".", ",");
            decimal productPrice = decimal.Parse(productPriceReplaced);
            int productCategory = int.Parse(Request.Form["inputCategories"]);
            string productCategoryText = Request.Form["inputCategoryText"];
            string productCategoryBgText = Request.Form["inputCategoryBgText"];
            HttpPostedFileBase productCategoryImg = CurrentSession.NewCategoryImage;
            bool productSpecials = int.Parse(Request.Form["inputSpecials"]) == 1 ? true : false; 
            string productPromotionPercentReplaced = Request.Form["inputPromotionPercent"];
            bool productStatus = int.Parse(Request.Form["inputStatus"]) == 1 ? true : false;
            bool categoryGender = int.Parse(Request.Form["inputCategoryGender"]) == 1 ? true : false;
            bool categoryStatus = int.Parse(Request.Form["inputCategoryStatus"]) == 1 ? true : false;
            productPromotionPercentReplaced = productPromotionPercentReplaced.Replace(".", ",");
            decimal productPromotionPercent = productSpecials == false ? 0 : decimal.Parse(productPromotionPercentReplaced);
            var productSize = Request.Form["inputListSize"].Split(',');
            var productQuantity = Request.Form["inputListQuantity"].Split(',');

            List<double> productListSize = new List<double>();
            List<int> productListQuantity = new List<int>();
            foreach (var sizeItem in productSize)
            {
                string sizeReplaced = sizeItem;
                sizeReplaced = sizeReplaced.Replace(".", ",");
                double tempSize = double.Parse(sizeReplaced);
                if (tempSize < 10 || tempSize > 55)
                {
                    return null;
                }
                productListSize.Add(tempSize);
            }

            foreach (var quantityItem in productQuantity)
            {
                int quantityParse = int.Parse(quantityItem);
                productListQuantity.Add(quantityParse);
            }

            string productCategoryImgName = null;

            if (productPromotionPercent < 0 || productPromotionPercent > 100)
            {
                return null;
            }
           
            string productDescription = Request.Form["inputDescription"];
            string productBgDescription = Request.Form["inputBgDescription"];

            if (string.IsNullOrEmpty(productName))
            {
                return null;
            }
            if (string.IsNullOrEmpty(productBgName))
            {
                return null;
            }
            
            if (productCategory == 0)
            {
                if (string.IsNullOrEmpty(productCategoryText))
                {
                    return null;
                }
                if (string.IsNullOrEmpty(productCategoryBgText))
                {
                    return null;
                }

                //string time = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                //  DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                //productCategoryImgName = time + "_" + Path.GetFileName(productCategoryImg.FileName);
                //var physicalPath = Path.Combine(Server.MapPath(directoryOfProductMainBody), productCategoryImgName);
                //productCategoryImg.SaveAs(physicalPath);

                string time = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                productCategoryImgName = time + "_" + Path.GetFileName(productCategoryImg.FileName);
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.PhotoInAdminPanel), productCategoryImgName);
                productCategoryImg.SaveAs(filePath);

                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(productCategoryImg.FileName);
                var fullPath = Path.Combine(Server.MapPath(Resources.Pictures.PhotoInAdminPanel), filenameT);
                productCategoryImg.SaveAs(fullPath);

                using (Image myImage = Image.FromStream(productCategoryImg.InputStream))
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

            }
            if (string.IsNullOrEmpty(productDescription))
            {
                return null;
            }
            if (string.IsNullOrEmpty(productBgDescription))
            {
                return null;
            }
            

            string productMainImgName = null;

            if (productMainImg != null)
            {
                //string productImgTime = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                //DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                //productMainImgName = productImgTime + "_" + Path.GetFileName(productMainImg.FileName);
                //var physicalPathMainProduct = Path.Combine(Server.MapPath(directoryOfProductMainBody), productMainImgName);
                //productMainImg.SaveAs(physicalPathMainProduct);

                string productImgTime = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                productMainImgName = productImgTime + "_" + Path.GetFileName(productMainImg.FileName);
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.PhotoInAdminPanel), productMainImgName);
                productMainImg.SaveAs(filePath);

                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(productMainImg.FileName);
                var fullPath = Path.Combine(Server.MapPath(Resources.Pictures.PhotoInAdminPanel), filenameT);
                productMainImg.SaveAs(fullPath);

                using (Image myImage = Image.FromStream(productMainImg.InputStream))
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

                if (productId == null)
                {
                    productId = "0";
                }
                int? productIdInt = int.Parse(productId);
                var productToEdit = db.Products.FirstOrDefault(w => w.Id == productIdInt);

                if(productToEdit != null)
                {
                    fullPath = Server.MapPath(Resources.Pictures.PhotoInAdminPanel + productToEdit.PhotoHeader);
                    if (System.IO.File.Exists(fullPath))
                    {

                        System.IO.File.Delete(fullPath);
                    }
                }
            }

            List<string> listProductBodyImg = new List<string>();

            if(productBodyImg != null)
            {
                foreach (var bodyImg in productBodyImg)
                {
                    //string productBodyImgTime = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                    // DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    //string productBodyImgName = productBodyImgTime + "_" + Path.GetFileName(bodyImg.FileName);
                    //var physicalPathBodyProduct = Path.Combine(Server.MapPath(directoryOfProductMainBody), productBodyImgName);
                    //bodyImg.SaveAs(physicalPathBodyProduct);

                    string productBodyImgTime = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                    DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    string productBodyImgName = productBodyImgTime + "_" + Path.GetFileName(bodyImg.FileName);
                    string filePath = Path.Combine(Server.MapPath(Resources.Pictures.PhotoInAdminPanel), productBodyImgName);
                    bodyImg.SaveAs(filePath);

                    var timeT = DateTime.Now.Year + DateTime.Now.Month +
                    DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                    var filenameT = timeT + "_T" + Path.GetFileName(bodyImg.FileName);
                    var fullPath = Path.Combine(Server.MapPath(Resources.Pictures.PhotoInAdminPanel), filenameT);
                    bodyImg.SaveAs(fullPath);

                    using (Image myImage = Image.FromStream(bodyImg.InputStream))
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

                    listProductBodyImg.Add(productBodyImgName);
                }
            }

            string success = productRepository.CreateNewProduct(productId, productName, productBgName, productMainImgName, listProductBodyImg, productPrice, productCategory, 
            productCategoryText, productCategoryBgText, productCategoryImgName, productSpecials, productPromotionPercent, productStatus, categoryGender, categoryStatus,
            productListSize, productListQuantity, productDescription, productBgDescription, User.Identity.Name);

            return Json(success, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult CreateEditCategory(IEnumerable<HttpPostedFileBase> files)
        {
            string categoryId = CurrentSession.CategoryAddOrEditCategoryId;

            string categoryText = Request.Form["inputCategoryText"];
            string categoryBgText = Request.Form["inputCategoryBgText"];
            HttpPostedFileBase categoryImg = CurrentSession.NewCategoryImage;
            bool categoryStatus = int.Parse(Request.Form["inputCategoryStatus"]) == 1 ? true : false;
            bool categoryGender = int.Parse(Request.Form["inputCategoryGender"]) == 1 ? true : false;

            if (string.IsNullOrEmpty(categoryText))
            {
                return null;
            }

            if (string.IsNullOrEmpty(categoryBgText))
            {
                return null;
            }
            string categoryImgName = null;

            if (categoryImg != null)
            {
                string time = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                                DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                categoryImgName = time + "_" + Path.GetFileName(categoryImg.FileName);
                string filePath = Path.Combine(Server.MapPath(Resources.Pictures.PhotoInAdminPanel), categoryImgName);
                categoryImg.SaveAs(filePath);

                var timeT = DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second.ToString();
                var filenameT = timeT + "_T" + Path.GetFileName(categoryImg.FileName);
                var fullPath = Path.Combine(Server.MapPath(Resources.Pictures.PhotoInAdminPanel), filenameT);
                categoryImg.SaveAs(fullPath);

                using (Image myImage = Image.FromStream(categoryImg.InputStream))
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
            }
            

            string success = categoryRepository.CreateNewCategory(categoryId, categoryText, categoryBgText, categoryImgName, categoryStatus, categoryGender, User.Identity.Name);

            return Json(success, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveMainImgProduct(IEnumerable<HttpPostedFileBase> files)
        {

            CurrentSession.ProductMainImage = files.FirstOrDefault();

            return Content("");
        }

        public ActionResult RemoveMainImgProduct(string[] fileNames)
        {

            CurrentSession.ProductMainImage = null;

           //var file = CurrentSession.ProductMainImage;

            //if (file != null)
            //{

            //    var fileName = Path.GetFileName(file.FileName);
            //    var physicalPath = Path.Combine(Server.MapPath(directoryOfProductMainBody), fileName);

            //    if (System.IO.File.Exists(physicalPath))
            //    {
            //        System.IO.File.Delete(physicalPath);
            //    }
            //}

            return Content("");
        }

        public ActionResult SaveBodyImgProduct(IEnumerable<HttpPostedFileBase> files)
        {

            CurrentSession.ProductBodyImages = files.ToList();

            return Content("");
        }

        public ActionResult RemoveBodyImgProduct(string[] fileNames)
        {
            var imgRemove = CurrentSession.ProductBodyImages.FirstOrDefault(w => w.FileName == fileNames[0]);

            if(imgRemove != null)
            {
                CurrentSession.ProductBodyImages.Remove(imgRemove);
            }
            
            return Content("");
        }

        public ActionResult SaveCategoryImgProduct(IEnumerable<HttpPostedFileBase> files)
        {

            CurrentSession.NewCategoryImage = files.FirstOrDefault();

            return Content("");
        }

        public ActionResult RemoveCategoryImgProduct(string[] fileNames)
        {
            CurrentSession.NewCategoryImage = null;

            return Content("");
        }

        [HttpPost]
        public ActionResult ClearCurrentSessionPhotos()
        {
            CurrentSession.ProductAddOrEditProductId = null;
            CurrentSession.ProductMainImage = null;
            CurrentSession.ProductBodyImages = null;
            CurrentSession.NewCategoryImage = null;

            return Content("");
        }

        // GET: Founder/ProductPanel
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("IndexPartial")]
        public ActionResult _Index()
        {
            IEnumerable<ProductViewModel> allProducts = new List<ProductViewModel>();

            allProducts = productRepository.GetAllProductsGrid();

            return PartialView("_Index", allProducts);
        }

        [HttpGet]
        [ActionName("DropdownProductCategories")]
        public ActionResult _DropdownProductCategories()
        {

            List<CategoryViewModel> allCategories = new List<CategoryViewModel>();

            allCategories = categoryRepository.GetDropdownCategoriesGrid();

            return PartialView("_DropdownProductCategories", allCategories);
        }

        [HttpPost]
        [ActionName("CategoriesGrid")]
        public ActionResult _CategoriesGrid()
        {

            List<CategoryViewModel> allCategories = new List<CategoryViewModel>();

            allCategories = categoryRepository.GetAllCategories();

            return PartialView("_CategoriesGrid", allCategories);
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
