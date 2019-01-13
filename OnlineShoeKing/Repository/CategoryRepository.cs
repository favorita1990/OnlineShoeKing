

namespace OnlineShoeKing.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Models;
    using Models.ViewModels;

    public class CategoryRepository
    {
        private readonly DBContext db = new DBContext();
        public UserRepository userRepository = new UserRepository();
        private string directoryToProductPhtoto = "/images/products/";

        public int QuantityProductByCategory(Category category) 
        {
            
            return new int();
        }

        public List<HomeCollectionsViewModel> GetCollections()
        {

            List<HomeCollectionsViewModel> collections = new List<HomeCollectionsViewModel>();

            var language = Resources.Language.BG == "БГ" ? true : false;

            var categories = db.Categories.Where(w => w.Status && w.Deleted == false).ToList();

            foreach (var category in db.Categories.ToList())
            {

                if (category.Status && category.Deleted == false)
                {
                    Random randNum = new Random();
                    int aRandomPos = randNum.Next(categories.Count);
                    var newCategory = categories[aRandomPos];
                    categories.Remove(newCategory);

                    collections.Add(
                    new HomeCollectionsViewModel
                    {
                        CollectionId = newCategory.Id,
                        Name = language ? newCategory.BgName : newCategory.Name,
                        Photo = directoryToProductPhtoto + newCategory.Photo
                    });

                    if (collections.Count == 3)
                    {
                        break;
                    }
                }
            }

            return collections;
        }

        public List<CollectionsViewModel> GetAllCollections()
        {

            List<CollectionsViewModel> collections = new List<CollectionsViewModel>();

            var categories = db.Categories.Where(w => w.Status && w.Deleted == false).ToList();

            foreach (var category in db.Categories.ToList())
            {
                if(category.Status && category.Deleted == false)
                {
                    int productCount = category.Products.Count;

                    string lastAdded = "No Products";

                    if (productCount > 0)
                    {
                        lastAdded = category.Products.OrderByDescending(o => o.Updated).ToList().FirstOrDefault().Updated.ToString();
                    }

                    //var newCategory = categories.OrderBy(s => Guid.NewGuid()).First();
                    Random randNum = new Random();
                    int aRandomPos = randNum.Next(categories.Count);
                    var newCategory = categories[aRandomPos];
                    categories.Remove(newCategory);

                    collections.Add(
                    new CollectionsViewModel
                    {
                        CollectionId = newCategory.Id,
                        Name = newCategory.Name,
                        Photo = directoryToProductPhtoto + newCategory.Photo,
                        ProductLastAdded = lastAdded,
                        ProductCount = productCount
                    });

                    if (collections.Count == 6)
                    {
                        break;
                    }
                }
            }

            return collections;
        }

        public Category find(int? id)
        {
            return db.Categories.Find(id);
        }

        public List<CategoryViewModel> GetAllCategories()
        {

            var categories = new List<CategoryViewModel>();
           
            foreach (var category in db.Categories.OrderBy(o => o.Deleted).ToList())
            {
                UserModel createdByUser = userRepository.GetUserById(category.CreatedBy);
                UserModel updatedByUser = userRepository.GetUserById(category.UpdatedBy);

                categories.Add(
                new CategoryViewModel
                {
                    CategoryId = category.Id,
                    Name = category.Name,
                    BgName = category.BgName,
                    Photo = directoryToProductPhtoto + category.Photo,
                    Created = category.Created.ToString("MM.dd.yyyy - HH:mm"),
                    CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}",
                    Updated = category.Updated.ToString("MM.dd.yyyy - HH:mm"),
                    UpdatedBy = $"{updatedByUser.FirstName} {updatedByUser.LastName}",
                    Deleted = category.Deleted ? "Yes" : "No",
                    Status = category.Status,
                    Gender = category.Gender,
                    GenderName = (bool)category.Gender ? "Men" : "Women",
                    QuantityProduct = category.Products.Count(w => w.Deleted == false)
                });
            }

            return categories;
        }

        public List<CategoryViewModel> GetWomenCategories()
        {

            var categories = new List<CategoryViewModel>();
            var categoriesDb = db.Categories?.Where(w => w.Gender == false).OrderBy(o => !o.Deleted).ToList();

            foreach (var category in categoriesDb)
            {
                var createdByUser = userRepository.GetUserById(category.CreatedBy);
                var updatedByUser = userRepository.GetUserById(category.UpdatedBy);

                var productsVm = new List<ProductViewModel>();
                var products = category.Products?.Where(w => (bool)w.Status && w.Deleted == false).OrderByDescending(o => o.Created).Take(4).ToList();
                foreach (var product in products)
                {
                    var sizeCount = 0;
                    foreach (var size in product?.SizeOfProducts.ToList())
                    {
                        if (size.Quantity > 0)
                        {
                            sizeCount++;
                        }
                    }

                    if (sizeCount > 0)
                    {
                        var promotionPrice = (decimal)product.Price;
                        if (product.Specials == true)
                        {
                            promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                        }

                        productsVm.Add(new ProductViewModel()
                                           {
                                               ProductId = product.Id,
                                               Name = product.Name,
                                               BgName = product.BgName,
                                               Price = (decimal)product.Price,
                                               PromotionPercent = (int)product.PromotionPercent,
                                               PromotionPrice = Math.Round(promotionPrice, 2),
                                               Specials = (bool)product.Specials,
                                               Status = (bool)product.Status,
                                               PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        });
                    }
                }
                categories.Add(
                    new CategoryViewModel
                        {
                            CategoryId = category.Id,
                            Name = category.Name,
                            BgName = category.BgName,
                            Photo = directoryToProductPhtoto + category.Photo,
                            Created = category.Created.ToString("MM.dd.yyyy - HH:mm"),
                            CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}",
                            Updated = category.Updated.ToString("MM.dd.yyyy - HH:mm"),
                            UpdatedBy = $"{updatedByUser.FirstName} {updatedByUser.LastName}",
                            Deleted = category.Deleted ? "Yes" : "No",
                            Status = category.Status,
                            QuantityProduct = category.Products?.Count(w => w.Deleted == false),
                            Products = productsVm
                    });
            }

            return categories;
        }

        public List<CategoryViewModel> GetMenCategories()
        {

            var categories = new List<CategoryViewModel>();
            var categoriesDb = db.Categories?.Where(w => w.Gender == true).OrderBy(o => !o.Deleted).ToList();

            foreach (var category in categoriesDb)
            {
                var createdByUser = userRepository.GetUserById(category.CreatedBy);
                var updatedByUser = userRepository.GetUserById(category.UpdatedBy);

                var productsVm = new List<ProductViewModel>();
                var products = category.Products?.Where(w => (bool)w.Status && w.Deleted == false).OrderByDescending(o => o.Created).Take(4).ToList();
                foreach (var product in products)
                {
                    var sizeCount = 0;
                    foreach (var size in product?.SizeOfProducts.ToList())
                    {
                        if (size.Quantity > 0)
                        {
                            sizeCount++;
                        }
                    }

                    if (sizeCount > 0)
                    {
                        var promotionPrice = (decimal)product.Price;
                        if (product.Specials == true)
                        {
                            promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                        }

                        productsVm.Add(new ProductViewModel()
                        {
                            ProductId = product.Id,
                            Name = product.Name,
                            BgName = product.BgName,
                            Price = (decimal)product.Price,
                            PromotionPercent = (int)product.PromotionPercent,
                            PromotionPrice = Math.Round(promotionPrice, 2),
                            Specials = (bool)product.Specials,
                            Status = (bool)product.Status,
                            PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        });
                    }
                }
                categories.Add(
                    new CategoryViewModel
                    {
                        CategoryId = category.Id,
                        Name = category.Name,
                        BgName = category.BgName,
                        Photo = directoryToProductPhtoto + category.Photo,
                        Created = category.Created.ToString("MM.dd.yyyy - HH:mm"),
                        CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}",
                        Updated = category.Updated.ToString("MM.dd.yyyy - HH:mm"),
                        UpdatedBy = $"{updatedByUser.FirstName} {updatedByUser.LastName}",
                        Deleted = category.Deleted ? "Yes" : "No",
                        Status = category.Status,
                        QuantityProduct = category.Products?.Count(w => w.Deleted == false),
                        Products = productsVm
                    });
            }

            return categories;
        }

        public List<CategoryViewModel> GetDropdownCategoriesGrid()
        {

            List<CategoryViewModel> categories = new List<CategoryViewModel>();

            foreach (var category in db.Categories.ToList())
            {
                if(category.Deleted == false && category.Status)
                {
                    UserModel createdByUser = userRepository.GetUserById(category.CreatedBy);
                    UserModel updatedByUser = userRepository.GetUserById(category.UpdatedBy);

                    categories.Add(
                    new CategoryViewModel
                    {
                        CategoryId = category.Id,
                        Name = category.Name,
                        Photo = directoryToProductPhtoto + category.Photo,
                        Created = category.Created.ToString("MM.dd.yyyy - HH:mm"),
                        CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}",
                        Updated = category.Updated.ToString("MM.dd.yyyy - HH:mm"),
                        UpdatedBy = $"{updatedByUser.FirstName} {updatedByUser.LastName}",
                        Deleted = category.Deleted ? "Yes" : "No",
                        Status = category.Status,
                        QuantityProduct = category.Products.Count(w => w.Deleted == false)
                    });
                }
            }

            return categories;
        }

        public CategoryViewModel GetCategoryByIdGridEdit(int categoryId)
        {
            
            var category = db.Categories.FirstOrDefault(w => w.Id == categoryId);
            UserModel createdByUser = userRepository.GetUserById(category.CreatedBy);
            UserModel updatedByUser = userRepository.GetUserById(category.UpdatedBy);

            CategoryViewModel categoryViewModel = new CategoryViewModel()
            {
                CategoryId = category.Id,
                Name = category.Name,
                BgName = category.BgName,
                Photo = directoryToProductPhtoto + category.Photo,
                Created = category.Created.ToString("MM.dd.yyyy - HH:mm"),
                CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}",
                Updated = category.Updated.ToString("MM.dd.yyyy - HH:mm"),
                UpdatedBy = $"{updatedByUser.FirstName} {updatedByUser.LastName}",
                Deleted = category.Deleted ? "Yes" : "No",
                Status = category.Status,
                Gender = category.Gender,
                GenderName = (bool)category.Gender ? "Men" : "Women",
                QuantityProduct = category.Products.Count(w => w.Deleted == false)
            };

            return categoryViewModel;
        }

        public List<ProductViewModel> GetProductByCategoryId(int categoryId)
        {
            
            List<ProductViewModel> productsViewModel = new List<ProductViewModel>();

            var productCategory = db.Products.Where(w => w.CategoryId == categoryId).ToList();

            foreach (var product in productCategory)
            {
                decimal promotionPrice = (decimal)product.Price;
                if (product.Specials == true)
                {
                    promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                }

                UserModel createdByUser = userRepository.GetUserById(product.CreatedBy);
                UserModel updatedByUser = userRepository.GetUserById(product.UpdatedBy);

                productsViewModel.Add(new ProductViewModel
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = (decimal)product.Price,
                    PromotionPercent = (int)product.PromotionPercent,
                    PromotionPrice = Math.Round(promotionPrice, 2),
                    SizesOfProduct = null,
                    Specials = (bool)product.Specials,
                    Status = (bool)product.Status,
                    PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                    PhotosOfProduct = null,
                    CategoryName = null,
                    DateAdded = product.Created.ToString("MM.dd.yyyy - HH:mm"),
                    DateUpdated = product.Updated.ToString("MM.dd.yyyy - HH:mm"),
                    CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}",
                    UpdatedBy = $"{updatedByUser.FirstName} {updatedByUser.LastName}",
                    Deleted = product.Deleted ? "Yes" : "No"
                });
            }

            return productsViewModel;
        }

        public List<CategoryViewModel> SortAllMenCategories(string sortBy)
        {
            var categories = new List<CategoryViewModel>();
            var categoriesDb = db.Categories?.Where(w => w.Gender == true).OrderBy(o => !o.Deleted).ToList();

            if (categoriesDb != null)
            {
                if (categoriesDb.Count > 0)
                {
                    switch (sortBy)
                    {
                        case "1":
                            categoriesDb = categoriesDb.OrderByDescending(o => o.Created).ToList(); break;
                        case "-1":
                            categoriesDb = categoriesDb.OrderBy(o => o.Created).ToList(); break;
                        case "2":
                            categoriesDb = categoriesDb.OrderByDescending(o => o.Products.Count).ToList(); break;
                        case "-2":
                            categoriesDb = categoriesDb.OrderBy(o => o.Products.Count).ToList(); break;
                        case "3":
                            categoriesDb = categoriesDb.OrderBy(o => o.Name).ToList(); break;
                        case "-3":
                            categoriesDb = categoriesDb.OrderByDescending(o => o.Name).ToList(); break;
                    }
                }
            }

            foreach (var category in categoriesDb)
            {
                var createdByUser = userRepository.GetUserById(category.CreatedBy);
                var updatedByUser = userRepository.GetUserById(category.UpdatedBy);

                var productsVm = new List<ProductViewModel>();
                var products = category.Products?.Where(w => (bool)w.Status && w.Deleted == false).OrderByDescending(o => o.Created).Take(4).ToList();
                foreach (var product in products)
                {
                    var sizeCount = 0;
                    foreach (var size in product?.SizeOfProducts.ToList())
                    {
                        if (size.Quantity > 0)
                        {
                            sizeCount++;
                        }
                    }

                    if (sizeCount > 0)
                    {
                        var promotionPrice = (decimal)product.Price;
                        if (product.Specials == true)
                        {
                            promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                        }

                        productsVm.Add(new ProductViewModel()
                        {
                            ProductId = product.Id,
                            Name = product.Name,
                            BgName = product.BgName,
                            Price = (decimal)product.Price,
                            PromotionPercent = (int)product.PromotionPercent,
                            PromotionPrice = Math.Round(promotionPrice, 2),
                            Specials = (bool)product.Specials,
                            Status = (bool)product.Status,
                            PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        });
                    }
                }
                categories.Add(
                    new CategoryViewModel
                    {
                        CategoryId = category.Id,
                        Name = category.Name,
                        BgName = category.BgName,
                        Photo = directoryToProductPhtoto + category.Photo,
                        Created = category.Created.ToString("MM.dd.yyyy - HH:mm"),
                        CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}",
                        Updated = category.Updated.ToString("MM.dd.yyyy - HH:mm"),
                        UpdatedBy = $"{updatedByUser.FirstName} {updatedByUser.LastName}",
                        Deleted = category.Deleted ? "Yes" : "No",
                        Status = category.Status,
                        QuantityProduct = category.Products?.Count(w => w.Deleted == false),
                        Products = productsVm
                    });
            }

            return categories;
        }

        public List<CategoryViewModel> SortAllWomenCategories(string sortBy)
        {
            var categories = new List<CategoryViewModel>();
            var categoriesDb = db.Categories?.Where(w => w.Gender == false).OrderBy(o => !o.Deleted).ToList();

            if (categoriesDb != null)
            {
                if (categoriesDb.Count > 0)
                {
                    switch (sortBy)
                    {
                        case "1":
                            categoriesDb = categoriesDb.OrderByDescending(o => o.Created).ToList(); break;
                        case "-1":
                            categoriesDb = categoriesDb.OrderBy(o => o.Created).ToList(); break;
                        case "2":
                            categoriesDb = categoriesDb.OrderByDescending(o => o.Products.Count).ToList(); break;
                        case "-2":
                            categoriesDb = categoriesDb.OrderBy(o => o.Products.Count).ToList(); break;
                        case "3":
                            categoriesDb = categoriesDb.OrderBy(o => o.Name).ToList(); break;
                        case "-3":
                            categoriesDb = categoriesDb.OrderByDescending(o => o.Name).ToList(); break;
                    }
                }
            }

            foreach (var category in categoriesDb)
            {
                var createdByUser = userRepository.GetUserById(category.CreatedBy);
                var updatedByUser = userRepository.GetUserById(category.UpdatedBy);

                var productsVm = new List<ProductViewModel>();
                var products = category.Products?.Where(w => (bool)w.Status && w.Deleted == false).OrderByDescending(o => o.Created).Take(4).ToList();
                foreach (var product in products)
                {
                    var sizeCount = 0;
                    foreach (var size in product?.SizeOfProducts.ToList())
                    {
                        if (size.Quantity > 0)
                        {
                            sizeCount++;
                        }
                    }

                    if (sizeCount > 0)
                    {
                        var promotionPrice = (decimal)product.Price;
                        if (product.Specials == true)
                        {
                            promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                        }

                        productsVm.Add(new ProductViewModel()
                        {
                            ProductId = product.Id,
                            Name = product.Name,
                            BgName = product.BgName,
                            Price = (decimal)product.Price,
                            PromotionPercent = (int)product.PromotionPercent,
                            PromotionPrice = Math.Round(promotionPrice, 2),
                            Specials = (bool)product.Specials,
                            Status = (bool)product.Status,
                            PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        });
                    }
                }
                categories.Add(
                    new CategoryViewModel
                    {
                        CategoryId = category.Id,
                        Name = category.Name,
                        BgName = category.BgName,
                        Photo = directoryToProductPhtoto + category.Photo,
                        Created = category.Created.ToString("MM.dd.yyyy - HH:mm"),
                        CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}",
                        Updated = category.Updated.ToString("MM.dd.yyyy - HH:mm"),
                        UpdatedBy = $"{updatedByUser.FirstName} {updatedByUser.LastName}",
                        Deleted = category.Deleted ? "Yes" : "No",
                        Status = category.Status,
                        QuantityProduct = category.Products?.Count(w => w.Deleted == false),
                        Products = productsVm
                    });
            }

            return categories;
        }

        public void DeleteCategoryById(int categoryId)
        {
            var category = db.Categories.FirstOrDefault(w => w.Id == categoryId);
            category.Deleted = true;
            db.SaveChanges();
        }

        public string CreateNewCategory(string categoryId, string categoryText, string categoryBgText, string categoryImgName, bool categoryStatus, bool categoryGender, string userName)
        {
            int? categoryIdInt = 0;
            try 
            {
                categoryIdInt = int.Parse(categoryId);
            }
            catch (Exception)
            {
                // ignored
            }

            UserModel currentUser = db.Users.FirstOrDefault(u => u.Email == userName);
            Category currentCategory = db.Categories.FirstOrDefault(u => u.Id == categoryIdInt);

            if (currentCategory == null)
            {
                Category newCategory = new Category()
                {
                    Name = categoryText,
                    BgName = categoryBgText,
                    Photo = categoryImgName,
                    Created = DateTime.Now,
                    CreatedBy = currentUser.Id,
                    Updated = DateTime.Now,
                    UpdatedBy = currentUser.Id,
                    Deleted = false,
                    Status = categoryStatus,
                    Gender = categoryGender
                };

                db.Categories.Add(newCategory);
                db.SaveChanges();

                return "Category was created.";
            }
            else
            {
                if(currentCategory.Deleted == false)
                {
                    currentCategory.Name = categoryText;
                    currentCategory.BgName = categoryBgText;
                    currentCategory.Status = categoryStatus; 
                    currentCategory.Gender = categoryGender;
                    currentCategory.Updated = DateTime.Now;
                    currentCategory.UpdatedBy = currentUser.Id;
                    currentCategory.Deleted = false;

                    db.SaveChanges();

                    return "Category was updated.";
                }
                else
                {
                    return "Category's been deleted.<br/>Cannot change it.";
                }
            }
        }
    }
}