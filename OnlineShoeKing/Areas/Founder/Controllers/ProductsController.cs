
namespace OnlineShoeKing.Areas.Founder.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Context;

    using Microsoft.AspNet.Identity;

    using Models;
    using Repository;

    [Authorize(Roles = "Founder")]
    public class ProductsController : Controller
    {
        private readonly DBContext db = new DBContext();
        private readonly ProductRepository productRepository = new ProductRepository();
        private readonly UserRepository userRepository = new UserRepository();
        private readonly CategoryRepository categoryRepository = new CategoryRepository();
        private readonly string directoryOfProductIco = Resources.Pictures.PicProducts;
        private readonly BreadCrumbRepository breadCrumbRepository = new BreadCrumbRepository();


        [HttpPost]
        public ActionResult AddRating(int rateNumber)
        {
            var userId = User.Identity.GetUserId();
            var productId = Convert.ToInt32(CurrentSession.ProductId);
            var rating = productRepository.AddRatingToProduct(productId, userId, rateNumber);

            return PartialView("_Rating", rating);
        }

        [HttpPost]
        public ActionResult GetRatings(int productId)
        {
            var userId = User.Identity.GetUserId();
            var rating = productRepository.GetRatings(productId, userId);

            return PartialView("_Rating", rating);
        }

        public ActionResult Search(string search, string priceFrom, string priceTo)
        {
            var priceFromD = 0M;
            decimal.TryParse(priceFrom, out priceFromD);

            var priceToD = 0M;
            decimal.TryParse(priceTo, out priceToD);

            if (string.IsNullOrEmpty(search) && priceFromD == 0M && priceToD == 0M)
            {
                return View("NotFoundProduct");
            }

            var priceToUrl = priceToD == 0M ? "" : priceToD.ToString();
            TempData["searchUrl"] = $"search={search}";
            TempData["priceFromUrl"] = $"priceFrom={priceFromD.ToString()}";
            TempData["priceToUrl"] = $"priceTo={priceToUrl}";

            var products = productRepository.SearchProducts(search, priceFromD, priceToD);

            return View(products);
        }

        [HttpPost]
        [ActionName("SearchPartial")]
        public ActionResult _SearchPartial(string search, string priceFrom, string priceTo)
        {
            var priceFromD = 0M;
            decimal.TryParse(priceFrom, out priceFromD);

            var priceToD = 0M;
            decimal.TryParse(priceTo, out priceToD);

            if (search == "" && priceFromD == 0M && priceToD == 0M)
            {
                return View("NotFoundProduct");
            }

            var priceToUrl = priceToD == 0M ? "" : priceToD.ToString();
            TempData["searchUrl"] = $"search={search}";
            TempData["priceFromUrl"] = $"priceFrom={priceFromD.ToString()}";
            TempData["priceToUrl"] = $"priceTo={priceToUrl}";

            var products = productRepository.SearchProducts(search, priceFromD, priceToD);

            return PartialView("_Search", products);
        }

        [HttpPost]
        public ActionResult AddComment(string text)
        {
            var productId = Convert.ToInt32(CurrentSession.ProductId);
            var userId = User.Identity.GetUserId();
            var comments = productRepository.AddAdminCommentToProduct(productId, userId, text);

            return PartialView("_Comments", comments);
        }

        [HttpPost]
        public ActionResult GetComments(int productId)
        {
            var comments = productRepository.GetAdminCommentsByProductId(productId);

            return PartialView("_Comments", comments);
        }

        [HttpPost]
        public ActionResult RemoveComment(int commentId)
        {
            var productId = Convert.ToInt32(CurrentSession.ProductId);
            var comments = productRepository.RemoveAdminCommentToProduct(productId, commentId);

            return PartialView("_Comments", comments);
        }

        [HttpGet]
        public ActionResult GetFavorites()
        {
            var productsId = CurrentSession.GetListProducts;
            var products = new List<ProductViewModel>();

            foreach (var productId in productsId)
            {
                var product = productRepository.GetProductById(productId);
                products.Add(product);
            }
            
            return PartialView("_Favorites", products);
        }

        [HttpPost]
        public ActionResult RemoveProductFavorites(int productId)
        {
            CurrentSession.GetListProducts.Remove(productId);
            var productsId = CurrentSession.GetListProducts;
            var products = new List<ProductViewModel>();

            foreach (var id in productsId)
            {
                var product = productRepository.GetProductById(id);
                products.Add(product);
            }

            return PartialView("_Favorites", products);
        }


        // GET: Products
        public ActionResult Product(int? id)
        {
            if (id != null)
            {
                var productId = Convert.ToInt32(id);
                var product = productRepository.find(productId);

                if (product != null)
                {
                    CurrentSession.ProductId = productId;

                    var productViewModel = productRepository.GetProductById(product.Id);

                    var controller = "";
                    var controllerName = "";
                    var controllerPartial = "";
                    if ((bool)productViewModel.CategoryGender)
                    {
                        controller = "/Founder/Products/Men";
                        controllerName = Resources.Language.Men;
                        controllerPartial = "/Founder/Products/MenPartial";
                    }
                    else
                    {
                        controller = "/Founder/Products/Women";
                        controllerName = Resources.Language.Women;
                        controllerPartial = "/Founder/Products/WomenPartial";
                    }

                    var action = $"/Founder/Products/Category/{productViewModel.CategoryId}";
                    var actionName = Resources.Language.CategoryTitle;
                    var actionPartial = $"/Founder/Products/CategoryPartial";
                    var actionId = productViewModel.CategoryId;
                    var idName = Resources.Language.ProductTitle;

                    TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, action, actionName, actionPartial, actionId, idName);

                    return View(productViewModel);
                }

                return this.View("NotFoundProduct");
            }

            return this.View("NotFoundProduct");
        }

        [HttpPost]
        [ActionName("ProductPartial")]
        public ActionResult _Product(int productId)
        {
            var product = productRepository.find(productId);
            var productViewModel = productRepository.GetProductById(product.Id);

            var controller = "";
            var controllerName = "";
            var controllerPartial = "";
            if ((bool)productViewModel.CategoryGender)
            {
                controller = "/Founder/Products/Men";
                controllerName = Resources.Language.Men;
                controllerPartial = "/Founder/Products/MenPartial";
            }
            else
            {
                controller = "/Founder/Products/Women";
                controllerName = Resources.Language.Women;
                controllerPartial = "/Founder/Products/WomenPartial";
            }

            var action = $"/Founder/Products/Category/{productViewModel.CategoryId}";
            var actionName = Resources.Language.CategoryTitle;
            var actionPartial = $"/Founder/Products/CategoryPartial";
            var actionId = productViewModel.CategoryId;
            var idName = Resources.Language.ProductTitle;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, action, actionName, actionPartial, actionId, idName);

            CurrentSession.ProductId = productId;

            return product == null ? null : this.PartialView("_Product", productViewModel);
        }


        // GET: Products
        public ActionResult Category(int? id)
        {

            var category = categoryRepository.find(id);

            if (category != null)
            {
                if (!category.Deleted)
                {
                    TempData["categoryId"] = id;
                    var products = productRepository.GetProductsByCategoryId(category.Id);

                    var controller = "";
                    var controllerName = "";
                    var controllerPartial = "";
                    if ((bool)category.Gender)
                    {
                        controller = "/Founder/Products/Men";
                        controllerName = Resources.Language.Men;
                        controllerPartial = "/Founder/Products/MenPartial";
                    }
                    else
                    {
                        controller = "/Founder/Products/Women";
                        controllerName = Resources.Language.Women;
                        controllerPartial = "/Founder/Products/WomenPartial";
                    }

                    var actionName = Resources.Language.CategoryTitle;

                    TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

                    return View(products);
                }
            
                return View("NotFoundProduct");
            }

            return View("NotFoundProduct");
        }

        [HttpPost]
        [ActionName("CategoryPartial")]
        public ActionResult _Category(int categoryId)
        {
            Category category = categoryRepository.find(categoryId);

            if (category == null)
                return null;

            var controller = "";
            var controllerName = "";
            var controllerPartial = "";
            if ((bool)category.Gender)
            {
                controller = "/Founder/Products/Men";
                controllerName = Resources.Language.Men;
                controllerPartial = "/Founder/Products/MenPartial";
            }
            else
            {
                controller = "/Founder/Products/Women";
                controllerName = Resources.Language.Women;
                controllerPartial = "/Founder/Products/WomenPartial";
            }

            var actionName = Resources.Language.CategoryTitle;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            TempData["categoryId"] = category.Id;

            var products = productRepository.GetProductsByCategoryId(category.Id);

            return PartialView("_Category", products);
        }

        [HttpPost]
        [ActionName("Users")]
        public ActionResult _Users()
        {
            var productId = Convert.ToInt32(CurrentSession.ProductId);
            var currentuserId = User.Identity.GetUserId();
            var users = userRepository.GetUsersByProductId(currentuserId, productId);

            return PartialView("_Users", users);
        }

        [HttpPost]
        [ActionName("UserProducts")]
        public ActionResult _Products()
        {
            var currentuserId = User.Identity.GetUserId();
            var products = productRepository.GetUserPurchasedProducts(currentuserId);

            return PartialView("_Products", products);
        }

        [HttpPost]
        [ActionName("CustomerProducts")]
        public ActionResult _CustomerProducts(string email)
        {
            var currentuserId = db.Users.FirstOrDefault(f => f.Email == email)?.Id;

            var products = productRepository.GetUserPurchasedProducts(currentuserId);
            return PartialView("_Products", products);
        }

        [HttpPost]
        public ActionResult SortMenCategories(string sortBy)
        {

            CurrentSession.SortBy = CurrentSession.SortBy == sortBy ? $"-{sortBy}" : sortBy;

            var categories = categoryRepository.SortAllMenCategories(CurrentSession.SortBy);

            var controllerName = Resources.Language.Men;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            TempData["sortBy"] = sortBy;

            return PartialView("_Men", categories);
        }

        [HttpPost]
        public ActionResult SortWomenCategories(string sortBy)
        {

            CurrentSession.SortBy = CurrentSession.SortBy == sortBy ? $"-{sortBy}" : sortBy;

            var categories = categoryRepository.SortAllWomenCategories(CurrentSession.SortBy);

            var controllerName = Resources.Language.Women;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            TempData["sortBy"] = sortBy;

            return PartialView("_Women", categories);
        }

        [HttpPost]
        public ActionResult SortProducts(int categoryId, string sortBy)
        {
            var category = this.categoryRepository.find(categoryId);

            if (category == null)
                return null;

            TempData["categoryId"] = category.Id;

            var controller = "";
            var controllerName = "";
            var controllerPartial = "";
            if ((bool)category.Gender)
            {
                controller = "/Founder/Products/Men";
                controllerName = Resources.Language.Men;
                controllerPartial = "/Founder/Products/MenPartial";
            }
            else
            {
                controller = "/Founder/Products/Women";
                controllerName = Resources.Language.Women;
                controllerPartial = "/Founder/Products/WomenPartial";
            }

            var actionName = Resources.Language.CategoryTitle;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(controller, controllerName, controllerPartial, null, actionName, null, null);

            CurrentSession.SortBy = CurrentSession.SortBy == sortBy ? $"-{sortBy}" : sortBy;

            var products = productRepository.SortAllCategoryProducts(category.Id, CurrentSession.SortBy);

            TempData["sortBy"] = sortBy;

            return PartialView("_Category", products);
        }

        [HttpPost]
        public ActionResult AddToFavorites(int productId)
        {
            CurrentSession.AddListProducts = productId;

            var productsId = CurrentSession.GetListProducts;
            var products = new List<ProductViewModel>();

            foreach (var id in productsId)
            {
                var product = productRepository.GetProductById(id);
                products.Add(product);
            }

            return PartialView("_Favorites", products);
        }

        public ActionResult Women()
        {
            var controllerName = Resources.Language.Women;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            var categories = this.categoryRepository.GetWomenCategories();
            return View(categories);
        }

        [HttpGet]
        [ActionName("WomenPartial")]
        public ActionResult _Women()
        {
            var controllerName = Resources.Language.Women;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            var categories = this.categoryRepository.GetWomenCategories();

            return PartialView("_Women", categories);
        }

        public ActionResult Men()
        {
            var controllerName = Resources.Language.Men;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            var categories = this.categoryRepository.GetMenCategories();

            return View(categories);
        }

        [HttpGet]
        [ActionName("MenPartial")]
        public ActionResult _Men()
        {
            var controllerName = Resources.Language.Men;

            TempData["breadCrumb"] = breadCrumbRepository.AddBreadCrumb(null, controllerName, null, null, null, null, null);

            var categories = this.categoryRepository.GetMenCategories();

            return PartialView("_Men", categories);
        }
    }
}
