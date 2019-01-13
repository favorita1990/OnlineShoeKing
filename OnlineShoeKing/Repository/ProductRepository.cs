

namespace OnlineShoeKing.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Models;

    public class ProductRepository
    {
        private readonly DBContext db = new DBContext();
        public UserRepository userRepository = new UserRepository();
        private readonly string directoryToProductPhtoto = Resources.Pictures.PhotoPathProducts;

        public List<Product> LatestProducts(int n)
        {
            try
            {
                return db.Products.OrderByDescending(p => p.Id).Take(n).ToList();
            }
            catch (Exception)
            {
                return new List<Product> { new Product { Name = "", Price = 1000, Specials = true, Status = true } };
            }
        }

        public List<Product> FeaturedProducts(int n)
        {
            try
            {
                return db.Products.Where(p => p.Specials == true).OrderByDescending(p => p.Id).Take(n).ToList();
            }
            catch (Exception)
            {
                return new List<Product> { new Product { Name = "", Price = 1000, Specials = true, Status = true } };
            }
        }

        public Product find(int id)
        {
            return db.Products.Find(id);
        }

        public List<Product> RalatedProducts(Product product, int n)
        {
            return db.Products.Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id).Take(n).ToList();
        }

        public List<Product> specialProducts()
        {
            return db.Products.Where(p => p.Specials == true).ToList();
        }

        public List<Product> LatestProducts()
        {
            return db.Products.OrderByDescending(p => p.Id).ToList();
        }

        public CommentVm MapCommentToCommentVm(Comment comment)
        {
            var commentVm = new CommentVm()
            {
                Id = comment.Id,
                Text = comment.Text,
                Email = comment.User.Email,
                CreatedAt = comment.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                FullName = $"{comment.User.FirstName} {comment.User.LastName}",
                Photo = comment.User.ImageUrl != null ? Resources.Pictures.FolderUsersImages + comment.User.ImageUrl :
                                          (comment.User.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman)
            };

            return commentVm;
        }

        public decimal? productPromo(Product product)
        {

            decimal promotionPrice = (decimal)product.Price;

            if (product.Specials == true)
            {
                promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));

                return Math.Round(promotionPrice, 2);
            }

            return Math.Round((decimal)product.Price, 2);
        }

        public int ProductQuantitySize(int sizeId)
        {

            var sizeOfProduct = db.SizeOfProducts.Find(sizeId);

            if (sizeOfProduct.Quantity > 0)
            {
                sizeOfProduct.Quantity--;

                return sizeOfProduct.Quantity;
            }
            else
            {
                return -1;
            }
        }

        public void UpdateSizeOfProductQantity(List<Cart> cartItems)
        {
            foreach (var item in cartItems)
            {
                var sizeCart = db.SizeOfProducts.Find(item.SizeId);
                db.SaveChanges();
            }
        }

        public decimal? productPromoSaved(Product product, decimal n = 0.9M)
        {
            if (product.Status == true)
            {
                if (product.PromotionPercent > 0)
                    return product.Price - (product.Price * (1.0M - (product.PromotionPercent / 100)));
                if (product.PromotionPercent <= 0.0M)
                    return product.Price - (product.Price * n);
            }
            return product.Price;
        }

        public List<HomeNewArrivalsViewModel> GetNewArrivals()
        {
            var newArrivals = new List<HomeNewArrivalsViewModel>();

            var language = Resources.Language.BG == "БГ" ? true : false;


            foreach (var product in db.Products?.Take(12).ToList())
            {
                var promotionPrice = (decimal)productPromo(product);

                foreach (var item in product.SizeOfProducts)
                {
                    if (item.Quantity > 0 && product.Status == true && product.Deleted == false &&
                    product.Category.Status && product.Category.Deleted == false)
                    {
                        SizeOfHomeProduct size = new SizeOfHomeProduct()
                        {
                            Size = item.Size,
                            SizeId = item.Id
                        };


                        newArrivals.Add(
                            new HomeNewArrivalsViewModel
                            {
                                Id = product.Id,
                                Name = language ? product.BgName : product.Name,
                                BgName = product.BgName,
                                Description = language ? product.BgDescription : product.Description,
                                BgDescription = product.BgDescription,
                                Price = (decimal)product.Price,
                                PromotionPercent = (int)product.PromotionPercent,
                                PromotionPrice = Math.Round(promotionPrice, 2),
                                SizeOfProduct = size,
                                Specials = (bool)product.Specials,
                                Status = (bool)product.Status,
                                Photo = directoryToProductPhtoto + product.PhotoHeader,
                                ProductId = product.Id
                            });
                        break;
                    }
                }
            }

            return newArrivals;
        }

        public List<HomeMostBoughtViewModel> GetMostBought()
        {

            var mostBought = new List<HomeMostBoughtViewModel>();

            var language = Resources.Language.BG == "БГ" ? true : false;

            var products = db.Products?.OrderByDescending(w => w.OrderDetails.Count).Take(12).ToList();

            foreach (var product in products)
            {

                var promotionPrice = (decimal)productPromo(product);

                foreach (var item in product.SizeOfProducts)
                {
                    if (item.Quantity > 0 && product.Status == true && product.Deleted == false &&
                    product.Category.Status && product.Category.Deleted == false)
                    {
                        var size = new SizeOfHomeProduct()
                        {
                            Size = item.Size,
                            SizeId = item.Id
                        };

                        mostBought.Add(
                            new HomeMostBoughtViewModel
                            {
                                Id = product.Id,
                                Name = language ? product.BgName : product.Name,
                                BgName = product.BgName,
                                Description = language ? product.BgDescription : product.Description,
                                Price = (decimal)product.Price,
                                PromotionPercent = (int)product.PromotionPercent,
                                PromotionPrice = Math.Round(promotionPrice, 2),
                                SizeOfProduct = size,
                                Specials = (bool)product.Specials,
                                Status = (bool)product.Status,
                                Photo = directoryToProductPhtoto + product.PhotoHeader,
                                ProductId = product.Id
                            });

                        break;
                    }
                }
            }

            return mostBought;
        }

        public List<ProductViewModel> GetMayLikeproductsByProductId(int productId)
        {
            var productsViewModel = new List<ProductViewModel>();

            var currentProduct = db.Products.FirstOrDefault(f => f.Id == productId);
            var products = currentProduct.Category.Products?.OrderByDescending(o => o.Id).Take(12).ToList();

            var language = Resources.Language.BG == "БГ" ? true : false;

            foreach (var product in products)
            {
                var categoryName = language ? product.Category.BgName : product.Category.Name;
                var categoryBgName = product.Category.BgName;

                var sizeCount = 0;
                List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
                foreach (var size in product.SizeOfProducts.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizesOfProduct.Add(new SizesOfProduct
                        {
                            SizeId = size.Id,
                            Size = size.Size
                        });

                        sizeCount++;
                    }
                }

                if ((sizeCount > 0) && product.Status == true && product.Deleted == false &&
                product.Category.Status && product.Category.Deleted == false)
                {
                    List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                    }

                    productsViewModel.Add(new ProductViewModel
                    {
                        ProductId = product.Id,
                        Name = language ? product.BgName : product.Name,
                        Description = language ? product.BgDescription : product.Description,
                        Price = (decimal)product.Price,
                        PromotionPercent = (int)product.PromotionPercent,
                        PromotionPrice = Math.Round(promotionPrice, 2),
                        SizesOfProduct = sizesOfProduct,
                        Specials = (bool)product.Specials,
                        Status = (bool)product.Status,
                        PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        PhotosOfProduct = photosOfProduct,
                        CategoryName = categoryName,
                        CategoryBgName = categoryBgName,
                        DateAdded = product.Created.ToString("MM.dd.yyyy - HH:mm"),
                        DateUpdated = product.Updated.ToString("MM.dd.yyyy - HH:mm")
                    });
                }
            }

            return productsViewModel;
        }

        public List<ProductViewModel> SearchProducts(string search, decimal priceFrom, decimal priceTo)
        {
            var productsViewModel = new List<ProductViewModel>();

            var language = Resources.Language.BG == "БГ" ? true : false;

            var products = new List<Product>();

            if (priceTo != 0)
            {
                foreach (var product in db.Products?.ToList())
                {
                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));

                        if (priceTo > priceFrom)
                        {
                            if ((product.Name.ToLower().Contains(search.ToLower()) && promotionPrice >= priceFrom && promotionPrice <= priceTo) ||
                               (product.BgName.ToLower().Contains(search.ToLower()) && promotionPrice >= priceFrom && promotionPrice <= priceTo))
                            {
                                products.Add(product);
                            }
                        }
                        else
                        {
                            if ((product.Name.ToLower().Contains(search.ToLower()) && promotionPrice >= priceFrom) ||
                                 (product.BgName.ToLower().Contains(search.ToLower()) && promotionPrice >= priceFrom))
                            {
                                products.Add(product);
                            }
                        }
                    }
                    else
                    {
                        if(priceTo > priceFrom)
                        {
                            if ((product.Name.ToLower().Contains(search.ToLower()) && product.Price >= priceFrom && product.Price <= priceTo) ||
                                (product.BgName.ToLower().Contains(search.ToLower()) && product.Price >= priceFrom && product.Price <= priceTo))
                            {
                                products.Add(product);
                            }
                        }
                        else
                        {
                            if ((product.Name.ToLower().Contains(search.ToLower()) && product.Price >= priceFrom) ||
                               (product.BgName.ToLower().Contains(search.ToLower()) && product.Price >= priceFrom))
                            {
                                products.Add(product);
                            }
                        }
                      
                    }
                }
            }
            else
            {
                foreach (var product in db.Products?.ToList())
                {
                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));

                        if ((product.Name.ToLower().Contains(search.ToLower()) && promotionPrice >= priceFrom) ||
                            (product.BgName.ToLower().Contains(search.ToLower()) && promotionPrice >= priceFrom))
                        {
                            products.Add(product);
                        }
                    }
                    else
                    {
                        if ((product.Name.ToLower().Contains(search.ToLower()) && product.Price >= priceFrom) ||
                            (product.BgName.ToLower().Contains(search.ToLower()) && product.Price >= priceFrom))
                        {
                            products.Add(product);
                        }
                    }
                }
            }

            foreach (var product in products.OrderByDescending(o => o.Created))
            {
                var categoryName = product.Category.Name;
                var categoryBgName = product.Category.BgName;

                var sizeCount = 0;
                foreach (var size in product.SizeOfProducts.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizeCount++;
                    }
                }

                if ((sizeCount > 0) && product.Status == true && product.Deleted == false &&
                product.Category.Status && product.Category.Deleted == false)
                {
                    List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                    }

                    productsViewModel.Add(new ProductViewModel
                    {
                        ProductId = product.Id,
                        Name = language ? product.BgName : product.Name,
                        BgName = product.BgName,
                        Price = (decimal)product.Price,
                        PromotionPercent = (int)product.PromotionPercent,
                        PromotionPrice = Math.Round(promotionPrice, 2),
                        Specials = (bool)product.Specials,
                        Status = (bool)product.Status,
                        PhotoHeader = directoryToProductPhtoto + product.PhotoHeader
                    });
                }
            }

            return productsViewModel;
        }

        public List<CommentVm> GetCommentsByProductId(int productId, string userId)
        {
            var commentsVm = new List<CommentVm>();

            var user = db.Users.FirstOrDefault(f => f.Id == userId);
            var currentProduct = db.Products.FirstOrDefault(f => f.Id == productId);
            var comments = currentProduct.Comments?.OrderByDescending(o => o.CreatedAt).ToList();

            foreach (var comment in comments)
            {
                var commentTemp = MapCommentToCommentVm(comment);

                if (user?.Id == comment.User.Id)
                {
                    commentTemp.OwnComment = true;
                }

                commentsVm.Add(commentTemp);
            }

            return commentsVm;
        }

        public List<CommentVm> GetAdminCommentsByProductId(int productId)
        {
            var commentsVm = new List<CommentVm>();

            var currentProduct = db.Products.FirstOrDefault(f => f.Id == productId);
            var comments = currentProduct.Comments?.OrderByDescending(o => o.CreatedAt).ToList();

            foreach (var comment in comments)
            {
                var commentTemp = MapCommentToCommentVm(comment);
                commentTemp.OwnComment = true;
                commentsVm.Add(commentTemp);
            }

            return commentsVm;
        }

        public RatingVm GetRatings(int productId, string userId)
        {
            var currentProduct = db.Products.FirstOrDefault(f => f.Id == productId);

            var ratingsGroup = currentProduct.Ratings.OrderBy(o => o.Number).GroupBy(g => g.Number)?.ToList();

            var ratingVm = new RatingVm();

            var sumRatings = 0;

            currentProduct.Ratings?.ToList().ForEach(f => sumRatings += f.Number);

            if(userId != null)
            {
                var user = db.Users.Find(userId);
                var userRating = currentProduct.Ratings.FirstOrDefault(f => f.User.Id == user.Id);
                ratingVm.OwnRating = userRating != null ? userRating.Number : 0;
            }
           
            ratingVm.Average = currentProduct.Ratings.Count != 0 ? Math.Round((decimal)sumRatings / currentProduct.Ratings.Count, 1).ToString() : "0";
            ratingVm.AllReviews = currentProduct.Ratings.Count.ToString();

            foreach (var ratingGroup in ratingsGroup)
            {
                var ratingNumTemp = ratingGroup.FirstOrDefault();
                var starPercent = Math.Round(((decimal)ratingGroup.Count() / (decimal)currentProduct.Ratings.Count) * 100M, 2);

                switch (ratingNumTemp.Number)
                {
                    case 1: { ratingVm.Star1Percent = starPercent.ToString(); ratingVm.Star1People = ratingGroup.Count().ToString(); break; }
                    case 2: { ratingVm.Star2Percent = starPercent.ToString(); ratingVm.Star2People = ratingGroup.Count().ToString(); break; }
                    case 3: { ratingVm.Star3Percent = starPercent.ToString(); ratingVm.Star3People = ratingGroup.Count().ToString(); break; }
                    case 4: { ratingVm.Star4Percent = starPercent.ToString(); ratingVm.Star4People = ratingGroup.Count().ToString(); break; }
                    case 5: { ratingVm.Star5Percent = starPercent.ToString(); ratingVm.Star5People = ratingGroup.Count().ToString(); break; }
                }
            }

            return ratingVm;
        }

        public ProductViewModel GetProductById(int productId)
        {
            var productViewModel = new ProductViewModel();
            var product = db.Products.FirstOrDefault(w => w.Id == productId);
            var language = Resources.Language.BG == "БГ" ? true : false;

            var categoryId = product.Category.Id.ToString();
            var categoryName = language ? product.Category.BgName : product.Category.Name;
            var categoryBgName = product.Category.BgName;
            var categoryPhoto = product.Category.Photo;
            var categoryGender = product.Category.Gender;

            var sizeCount = 0;
            var sizesOfProduct = new List<SizesOfProduct>();
            foreach (var size in product.SizeOfProducts.ToList())
            {
                if (size.Quantity > 0)
                {
                    sizesOfProduct.Add(new SizesOfProduct
                    {
                        SizeId = size.Id,
                        Size = size.Size,
                        Quantity = size.Quantity
                    });

                    sizeCount++;
                }
            }

            if (sizeCount > 0)
            {
                List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                decimal promotionPrice = (decimal)product.Price;
                if (product.Specials == true)
                {
                    promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                }

                productViewModel.ProductId = product.Id;
                productViewModel.Name = language ? product.BgName : product.Name;
                productViewModel.BgName = product.BgName;
                productViewModel.Description = language ? product.BgDescription : product.Description;
                productViewModel.BgDescription = product.BgDescription;
                productViewModel.Price = (decimal)product.Price;
                productViewModel.PurchasedProductsCount = product.OrderDetails.Count;
                productViewModel.PromotionPercent = (int)product.PromotionPercent;
                productViewModel.PromotionPrice = Math.Round(promotionPrice, 2);
                productViewModel.SizesOfProduct = sizesOfProduct;
                productViewModel.Specials = (bool)product.Specials;
                productViewModel.Status = (bool)product.Status;
                productViewModel.PhotoHeader = directoryToProductPhtoto + product.PhotoHeader;
                productViewModel.PhotosOfProduct = photosOfProduct;
                productViewModel.CategoryId = categoryId;
                productViewModel.CategoryName = categoryName;
                productViewModel.CategoryBgName = categoryBgName;
                productViewModel.CategoryPhoto = categoryPhoto;
                productViewModel.CategoryGender = categoryGender;
            }

            return productViewModel;
        }

        public ProductViewModel GetProductSearchById(int productId)
        {
            ProductViewModel productViewModel = new ProductViewModel();

            var product = db.Products.FirstOrDefault(w => w.Id == productId);
            var language = Resources.Language.BG == "БГ" ? true : false;

            string categoryId = product.Category.Id.ToString();
            string categoryName = language ? product.Category.BgName : product.Category.Name;
            string categoryPhoto = product.Category.Photo;

            int sizeCount = 0;
            List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
            foreach (var size in product.SizeOfProducts.ToList())
            {
                if (size.Quantity > 0)
                {
                    sizesOfProduct.Add(new SizesOfProduct
                    {
                        SizeId = size.Id,
                        Size = size.Size,
                        Quantity = size.Quantity
                    });

                    sizeCount++;
                }
            }

            if (sizeCount > 0 && product.Status == true && (product.Deleted == false) &&
                product.Category.Status && product.Category.Deleted == false)
            {
                List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                decimal promotionPrice = (decimal)product.Price;
                if (product.Specials == true)
                {
                    promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                }

                productViewModel.ProductId = product.Id;
                productViewModel.Name = language ? product.BgName : product.Name;
                productViewModel.Description = language ? product.BgDescription : product.Description;
                productViewModel.Price = (decimal)product.Price;
                productViewModel.PromotionPercent = (int)product.PromotionPercent;
                productViewModel.PromotionPrice = Math.Round(promotionPrice, 2);
                productViewModel.SizesOfProduct = sizesOfProduct;
                productViewModel.Specials = (bool)product.Specials;
                productViewModel.Status = (bool)product.Status;
                productViewModel.PhotoHeader = directoryToProductPhtoto + product.PhotoHeader;
                productViewModel.PhotosOfProduct = photosOfProduct;
                productViewModel.CategoryId = categoryId;
                productViewModel.CategoryName = categoryName;
                productViewModel.CategoryPhoto = categoryPhoto;
            }

            return productViewModel;
        }

        public ProductViewModel GetProductByIdGridEdit(int productId)
        {
            var productViewModel = new ProductViewModel();
            var language = Resources.Language.BG == "БГ" ? true : false;

            var product = db.Products.FirstOrDefault(w => w.Id == productId);

            var categoryId = product.Category.Id.ToString();
            var categoryName = language ? product.Category.BgName : product.Category.Name;
            var categoryBgName = product.Category.BgName;
            var categoryPhoto = product.Category.Photo;

            var sizeCount = 0;
            var sizesOfProduct = new List<SizesOfProduct>();
            foreach (var size in product.SizeOfProducts.ToList())
            {
                if (size.Quantity > -1)
                {
                    sizesOfProduct.Add(new SizesOfProduct
                    {
                        SizeId = size.Id,
                        Size = size.Size,
                        Quantity = size.Quantity
                    });

                    sizeCount++;
                }
            }

            if (sizeCount > 0)
            {
                List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                decimal promotionPrice = (decimal)product.Price;
                if (product.Specials == true)
                {
                    promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                }

                productViewModel.ProductId = product.Id;
                productViewModel.Name = language ? product.BgName : product.Name;
                productViewModel.BgName = product.BgName;
                productViewModel.Description = language ? product.BgDescription : product.Description;
                productViewModel.BgDescription = product.BgDescription;
                productViewModel.Price = (decimal)product.Price;
                productViewModel.PromotionPercent = (int)product.PromotionPercent;
                productViewModel.PromotionPrice = Math.Round(promotionPrice, 2);
                productViewModel.SizesOfProduct = sizesOfProduct;
                productViewModel.Specials = (bool)product.Specials;
                productViewModel.Status = (bool)product.Status;
                productViewModel.PhotoHeader = directoryToProductPhtoto + product.PhotoHeader;
                productViewModel.PhotosOfProduct = photosOfProduct;
                productViewModel.CategoryId = categoryId;
                productViewModel.CategoryName = categoryName;
                productViewModel.CategoryBgName = categoryBgName;
                productViewModel.CategoryPhoto = categoryPhoto;
            }

            return productViewModel;
        }

        public List<ProductViewModel> GetProductsByOrderId(int orderId)
        {
            List<ProductViewModel> productsViewModel = new List<ProductViewModel>();
            var language = Resources.Language.BG == "БГ" ? true : false;

            var orderDetails = db.OrderDetails.Where(w => w.OrderId == orderId).ToList();

            foreach (var orderDetail in orderDetails)
            {
                string categoryName = language ? orderDetail.Product.Category.BgName : orderDetail.Product.Category.Name;

                int sizeCount = 0;
                List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
                foreach (var size in orderDetail.Product.SizeOfProducts.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizesOfProduct.Add(new SizesOfProduct
                        {
                            SizeId = size.Id,
                            Size = size.Size
                        });

                        sizeCount++;
                    }
                }

                List<string> photosOfProduct = orderDetail.Product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                decimal promotionPrice = (decimal)orderDetail.Product.Price;
                if (orderDetail.Product.Specials == true)
                {
                    promotionPrice = (decimal)orderDetail.Product.Price * (1.0M - (orderDetail.Product.PromotionPercent / 100));
                }

                productsViewModel.Add(new ProductViewModel
                {
                    ProductId = orderDetail.Product.Id,
                    Name = language ? orderDetail.Product.BgName : orderDetail.Product.Name,
                    Description = language ? orderDetail.Product.BgDescription : orderDetail.Product.Description,
                    Price = (decimal)orderDetail.Product.Price,
                    PromotionPercent = (int)orderDetail.Product.PromotionPercent,
                    PromotionPrice = Math.Round(promotionPrice, 2),
                    SizesOfProduct = sizesOfProduct,
                    Specials = (bool)orderDetail.Product.Specials,
                    Status = (bool)orderDetail.Product.Status,
                    PhotoHeader = directoryToProductPhtoto + orderDetail.Product.PhotoHeader,
                    PhotosOfProduct = photosOfProduct,
                    CategoryName = categoryName,
                    DateAdded = orderDetail.Product.Created.ToString("MM.dd.yyyy - HH:mm"),
                    DateUpdated = orderDetail.Product.Updated.ToString("MM.dd.yyyy - HH:mm"),
                    Deleted = orderDetail.Product.Deleted ? Resources.Language.YesTitle : Resources.Language.NoTitle
                });
            }

            return productsViewModel;
        }

        public void DeleteProductById(int productId)
        {
            var product = db.Products.FirstOrDefault(w => w.Id == productId);
            product.Deleted = true;
            db.SaveChanges();
        }

        public List<ProductViewModel> GetProductsByCategoryId(int categoryId)
        {
            var productsViewModel = new List<ProductViewModel>();

            var products = db.Products.Where(w => w.CategoryId == categoryId).OrderByDescending(o => o.Created).ToList();

            var language = Resources.Language.BG == "БГ" ? true : false;

            foreach (var product in products)
            {
                var categoryName = language ? product.Category.BgName : product.Category.Name;
                var categoryBgName = product.Category.BgName;

                var sizeCount = 0;
                List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
                foreach (var size in product.SizeOfProducts.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizesOfProduct.Add(new SizesOfProduct
                        {
                            SizeId = size.Id,
                            Size = size.Size
                        });

                        sizeCount++;
                    }
                }

                if ((sizeCount > 0) && product.Status == true && product.Deleted == false &&
                product.Category.Status && product.Category.Deleted == false)
                {
                    List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                    }

                    productsViewModel.Add(new ProductViewModel
                    {
                        ProductId = product.Id,
                        Name = language ? product.BgName : product.Name,
                        Description = language ? product.BgDescription : product.Description,
                        Price = (decimal)product.Price,
                        PromotionPercent = (int)product.PromotionPercent,
                        PromotionPrice = Math.Round(promotionPrice, 2),
                        SizesOfProduct = sizesOfProduct,
                        Specials = (bool)product.Specials,
                        Status = (bool)product.Status,
                        PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        PhotosOfProduct = photosOfProduct,
                        CategoryName = categoryName,
                        CategoryBgName = categoryBgName,
                        DateAdded = product.Created.ToString("MM.dd.yyyy - HH:mm"),
                        DateUpdated = product.Updated.ToString("MM.dd.yyyy - HH:mm")
                    });
                }
            }

            return productsViewModel;
        }

        public List<SizesOfProduct> GetSizeOfProductByProductId(int productId)
        {
            List<SizesOfProduct> sizeOfProductVM = new List<SizesOfProduct>();

            var sizeOfProduct = db.SizeOfProducts.Where(w => w.ProductId == productId).ToList();

            foreach (var item in sizeOfProduct)
            {
                sizeOfProductVM.Add(new SizesOfProduct
                {
                    Size = item.Size,
                    SizeId = item.Id,
                    Quantity = item.Quantity
                });
            }

            return sizeOfProductVM;
        }

        public List<PhotoOfProductVM> GetPhotoOfProductByProductId(int productId)
        {
            List<PhotoOfProductVM> photoOfProductVM = new List<PhotoOfProductVM>();

            var photoOfProduct = db.PhotoOfProducts.Where(w => w.ProductId == productId).ToList();

            foreach (var item in photoOfProduct)
            {
                photoOfProductVM.Add(new PhotoOfProductVM
                {
                    PhotoId = item.Id,
                    Photo = item.Photo,
                    ProductId = item.ProductId
                });
            }

            return photoOfProductVM;
        }

        public List<ProductViewModel> GetAllProducts()
        {
            List<ProductViewModel> productsViewModel = new List<ProductViewModel>();

            var products = db.Products.ToList();

            var language = Resources.Language.BG == "БГ" ? true : false;

            foreach (var product in products)
            {
                string categoryName = language ? product.Category.BgName : product.Category.Name;

                int sizeCount = 0;
                List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
                foreach (var size in product.SizeOfProducts.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizesOfProduct.Add(new SizesOfProduct
                        {
                            SizeId = size.Id,
                            Size = size.Size
                        });

                        sizeCount++;
                    }
                }

                if ((sizeCount > 0) && (product.Status == true) && (product.Deleted == false) &&
                product.Category.Status && (product.Category.Deleted == false))
                {
                    List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                    }

                    productsViewModel.Add(new ProductViewModel
                    {
                        ProductId = product.Id,
                        Name = language ? product.BgName : product.Name,
                        Description = language ? product.BgDescription : product.Description,
                        Price = (decimal)product.Price,
                        PromotionPercent = (int)product.PromotionPercent,
                        PromotionPrice = Math.Round(promotionPrice, 2),
                        SizesOfProduct = sizesOfProduct,
                        Specials = (bool)product.Specials,
                        Status = (bool)product.Status,
                        PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        PhotosOfProduct = photosOfProduct,
                        CategoryName = categoryName,
                        DateAdded = product.Created.ToString("MM.dd.yyyy - HH:mm"),
                        DateUpdated = product.Updated.ToString("MM.dd.yyyy - HH:mm")
                    });
                }
            }

            return productsViewModel;
        }

        public List<ProductViewModel> GetUserPurchasedProducts(string userId)
        {
            var user = db.Users.Find(userId);
            var userOrders = user.Orders.Where(w => w.OrderStatusId == 2);
            List<ProductViewModel> productsViewModel = new List<ProductViewModel>();
            var language = Resources.Language.BG == "БГ" ? true : false;

            foreach (var order in userOrders.OrderByDescending(o => o.Id))
            {
                foreach (var orderDetailGroup in order.OrderDetails.GroupBy(g => g.ProductId))
                {
                    var orderDetail = orderDetailGroup.FirstOrDefault();

                    string categoryName = language ? orderDetail.Product.Category.BgName : orderDetail.Product.Category.Name;

                    int sizeCount = 0;
                    List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
                    foreach (var size in orderDetail.Product.SizeOfProducts.ToList())
                    {
                        if (size.Quantity > 0)
                        {
                            sizesOfProduct.Add(new SizesOfProduct
                            {
                                SizeId = size.Id,
                                Size = size.Size
                            });

                            sizeCount++;
                        }
                    }

                    if ((sizeCount > 0) && (orderDetail.Product.Status == true) && (orderDetail.Product.Deleted == false) &&
                    orderDetail.Product.Category.Status && (orderDetail.Product.Category.Deleted == false))
                    {
                        List<string> photosOfProduct = orderDetail.Product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                        decimal promotionPrice = (decimal)orderDetail.Product.Price;
                        if (orderDetail.Product.Specials == true)
                        {
                            promotionPrice = (decimal)orderDetail.Product.Price * (1.0M - (orderDetail.Product.PromotionPercent / 100));
                        }

                        productsViewModel.Add(new ProductViewModel
                        {
                            ProductId = orderDetail.Product.Id,
                            Name = language ? orderDetail.Product.BgName : orderDetail.Product.Name,
                            BgName = orderDetail.Product.BgName,
                            Description = language ? orderDetail.Product.BgDescription : orderDetail.Product.Description,
                            BgDescription = orderDetail.Product.Description,
                            PurchasedProductsCount = orderDetailGroup.Count(),
                            Price = (decimal)orderDetail.Product.Price,
                            PromotionPercent = (int)orderDetail.Product.PromotionPercent,
                            PromotionPrice = Math.Round(promotionPrice, 2),
                            SizesOfProduct = sizesOfProduct,
                            Specials = (bool)orderDetail.Product.Specials,
                            Status = (bool)orderDetail.Product.Status,
                            PhotoHeader = directoryToProductPhtoto + orderDetail.Product.PhotoHeader,
                            PhotosOfProduct = photosOfProduct,
                            CategoryName = categoryName,
                            DateAdded = order.Created.Value.ToString("dd.MM.yyyy - HH:mm"),
                            DateUpdated = orderDetail.Product.Updated.ToString("dd.MM.yyyy - HH:mm")
                        });
                    }
                }
            }

            return productsViewModel;
        }

        public List<ProductViewModel> GetAllProductsGrid()
        {
            List<ProductViewModel> productsViewModel = new List<ProductViewModel>();

            var products = db.Products.OrderBy(o => o.Deleted).ToList();
            var language = Resources.Language.BG == "БГ" ? true : false;

            foreach (var product in products)
            {
                string categoryName = language ? product.Category.BgName : product.Category.Name;

                int sizeCount = 0;
                List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
                foreach (var size in product.SizeOfProducts?.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizesOfProduct.Add(new SizesOfProduct
                        {
                            SizeId = size.Id,
                            Size = size.Size
                        });

                        sizeCount++;
                    }
                }

                if (sizeCount > 0)
                {
                    UserModel createdByUser = userRepository.GetUserById(product.CreatedBy);
                    UserModel updatedByUser = userRepository.GetUserById(product.UpdatedBy);
                    List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                    }

                    productsViewModel.Add(new ProductViewModel
                    {
                        ProductId = product.Id,
                        Name = language ? product.BgName : product.Name,
                        BgName = product.BgName,
                        Description = language ? product.BgDescription : product.Description,
                        BgDescription = product.BgDescription,
                        Price = (decimal)product.Price,
                        PromotionPercent = (int)product.PromotionPercent,
                        PromotionPrice = Math.Round(promotionPrice, 2),
                        SizesOfProduct = sizesOfProduct,
                        Specials = (bool)product.Specials,
                        Status = (bool)product.Status,
                        PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        PhotosOfProduct = photosOfProduct,
                        CategoryName = categoryName,
                        DateAdded = product.Created.ToString("MM.dd.yyyy - HH:mm"),
                        DateUpdated = product.Updated.ToString("MM.dd.yyyy - HH:mm"),
                        CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}",
                        UpdatedBy = $"{updatedByUser.FirstName} {updatedByUser.LastName}",
                        Deleted = product.Deleted ? Resources.Language.YesTitle : Resources.Language.NoTitle
                    });
                }
            }

            return productsViewModel;
        }

        public List<ProductViewModel> GetAllDiscountProducts()
        {
            List<ProductViewModel> productsViewModel = new List<ProductViewModel>();

            var language = Resources.Language.BG == "БГ" ? true : false;

            var products = db.Products.Where(w => w.Specials == true).ToList();

            foreach (var product in products)
            {
                string categoryName = language ? product.Category.BgName : product.Category.Name;

                int sizeCount = 0;
                List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
                foreach (var size in product.SizeOfProducts.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizesOfProduct.Add(new SizesOfProduct
                        {
                            SizeId = size.Id,
                            Size = size.Size
                        });

                        sizeCount++;
                    }
                }

                if ((sizeCount > 0) && (product.Status == true) && (product.Deleted == false) &&
                (product.Category.Status) && (product.Category.Deleted == false))
                {
                    List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100M));
                    }

                    productsViewModel.Add(new ProductViewModel
                    {
                        ProductId = product.Id,
                        Name = language ? product.BgName : product.Name,
                        Description = language ? product.BgDescription : product.Description,
                        Price = (decimal)product.Price,
                        PromotionPercent = (int)product.PromotionPercent,
                        PromotionPrice = Math.Round(promotionPrice, 2),
                        SizesOfProduct = sizesOfProduct,
                        Specials = (bool)product.Specials,
                        Status = (bool)product.Status,
                        PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        PhotosOfProduct = photosOfProduct,
                        CategoryName = categoryName,
                        DateAdded = product.Created.ToString("MM.dd.yyyy - HH:mm"),
                        DateUpdated = product.Updated.ToString("MM.dd.yyyy - HH:mm")
                    });
                }
            }

            return productsViewModel;
        }

        public List<ProductViewModel> SortAllProducts(string sortBy)
        {
            List<ProductViewModel> productsViewModel = new List<ProductViewModel>();

            var language = Resources.Language.BG == "БГ" ? true : false;

            var products = db.Products.ToList();

            foreach (var product in products)
            {
                string categoryName = language ? product.Category.BgName : product.Category.Name;

                int sizeCount = 0;
                List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
                foreach (var size in product.SizeOfProducts.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizesOfProduct.Add(new SizesOfProduct
                        {
                            SizeId = size.Id,
                            Size = size.Size
                        });

                        sizeCount++;
                    }
                }

                if ((sizeCount > 0) && (product.Status == true) && (product.Deleted == false) &&
                product.Category.Status && (product.Category.Deleted == false))
                {
                    List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                    }

                    productsViewModel.Add(new ProductViewModel
                    {
                        ProductId = product.Id,
                        Name = language ? product.BgName : product.Name,
                        Description = language ? product.BgDescription : product.Description,
                        Price = (decimal)product.Price,
                        PromotionPercent = (int)product.PromotionPercent,
                        PromotionPrice = Math.Round(promotionPrice, 2),
                        SizesOfProduct = sizesOfProduct,
                        Specials = (bool)product.Specials,
                        Status = (bool)product.Status,
                        PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        PhotosOfProduct = photosOfProduct,
                        CategoryName = categoryName,
                        DateAdded = product.Created.ToString("MM.dd.yyyy - HH:mm"),
                        DateUpdated = product.Updated.ToString("MM.dd.yyyy - HH:mm")
                    });
                }
            }


            if (productsViewModel.Count > 0)
            {
                switch (sortBy)
                {
                    case "1":
                        productsViewModel = productsViewModel.OrderBy(o => o.DateAdded).ToList(); break;
                    case "-1":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.DateAdded).ToList(); break;
                    case "2":
                        productsViewModel = productsViewModel.OrderBy(o => o.Specials == true ? o.PromotionPrice : o.Price).ToList(); break;
                    case "-2":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.Specials == true ? o.PromotionPrice : o.Price).ToList(); break;
                    case "3":
                        productsViewModel = productsViewModel.OrderBy(o => o.Name).ToList(); break;
                    case "-3":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.Name).ToList(); break;
                }
            }

            return productsViewModel;
        }

        public List<ProductViewModel> SortAllCategoryProducts(int categoryId, string sortBy)
        {
            var productsViewModel = new List<ProductViewModel>();

            var language = Resources.Language.BG == "БГ" ? true : false;

            var products = db.Products.Where(w => w.CategoryId == categoryId).ToList();

            foreach (var product in products)
            {
                var categoryName = language ? product.Category.BgName : product.Category.Name;
                var categoryBgName = product.Category.BgName;

                var sizeCount = 0;
                var sizesOfProduct = new List<SizesOfProduct>();
                foreach (var size in product.SizeOfProducts.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizesOfProduct.Add(new SizesOfProduct
                        {
                            SizeId = size.Id,
                            Size = size.Size
                        });

                        sizeCount++;
                    }
                }

                if ((sizeCount > 0) && (product.Status == true) && (product.Deleted == false) &&
                product.Category.Status && (product.Category.Deleted == false))
                {
                    List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                    }

                    productsViewModel.Add(new ProductViewModel
                    {
                        ProductId = product.Id,
                        Name = language ? product.BgName : product.Name,
                        BgName = product.BgName,
                        Description = language ? product.BgDescription : product.Description,
                        BgDescription = product.BgDescription,
                        Price = (decimal)product.Price,
                        PromotionPercent = (int)product.PromotionPercent,
                        PromotionPrice = Math.Round(promotionPrice, 2),
                        SizesOfProduct = sizesOfProduct,
                        Specials = (bool)product.Specials,
                        Status = (bool)product.Status,
                        PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        PhotosOfProduct = photosOfProduct,
                        CategoryName = categoryName,
                        CategoryBgName = categoryBgName,
                        DateAdded = product.Created.ToString("MM.dd.yyyy - HH:mm"),
                        DateUpdated = product.Updated.ToString("MM.dd.yyyy - HH:mm")
                    });
                }
            }

            if (productsViewModel.Count > 0)
            {
                switch (sortBy)
                {
                    case "1":
                        productsViewModel = productsViewModel.OrderBy(o => o.DateAdded).ToList(); break;
                    case "-1":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.DateAdded).ToList(); break;
                    case "2":
                        productsViewModel = productsViewModel.OrderBy(o => o.Specials == true ? o.PromotionPrice : o.Price).ToList(); break;
                    case "-2":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.Specials == true ? o.PromotionPrice : o.Price).ToList(); break;
                    case "3":
                        productsViewModel = productsViewModel.OrderBy(o => o.Name).ToList(); break;
                    case "-3":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.Name).ToList(); break;
                }
            }

            return productsViewModel;
        }

        public List<ProductViewModel> SortAllDiscountProducts(string sortBy)
        {
            List<ProductViewModel> productsViewModel = new List<ProductViewModel>();

            var products = db.Products.Where(w => w.Specials == true).ToList();

            var language = Resources.Language.BG == "БГ" ? true : false;

            foreach (var product in products)
            {
                string categoryName = language ? product.Category.BgName : product.Category.Name;

                int sizeCount = 0;
                List<SizesOfProduct> sizesOfProduct = new List<SizesOfProduct>();
                foreach (var size in product.SizeOfProducts.ToList())
                {
                    if (size.Quantity > 0)
                    {
                        sizesOfProduct.Add(new SizesOfProduct
                        {
                            SizeId = size.Id,
                            Size = size.Size
                        });

                        sizeCount++;
                    }
                }

                if ((sizeCount > 0) && (product.Status == true) && (product.Deleted == false) &&
                product.Category.Status && (product.Category.Deleted == false))
                {
                    List<string> photosOfProduct = product.PhotoOfProducts.Select(s => directoryToProductPhtoto + s.Photo).ToList();

                    decimal promotionPrice = (decimal)product.Price;
                    if (product.Specials == true)
                    {
                        promotionPrice = (decimal)product.Price * (1.0M - (product.PromotionPercent / 100));
                    }

                    productsViewModel.Add(new ProductViewModel
                    {
                        ProductId = product.Id,
                        Name = language ? product.BgName : product.Name,
                        Description = language ? product.BgDescription : product.Description,
                        Price = (decimal)product.Price,
                        PromotionPercent = (int)product.PromotionPercent,
                        PromotionPrice = Math.Round(promotionPrice, 2),
                        SizesOfProduct = sizesOfProduct,
                        Specials = (bool)product.Specials,
                        Status = (bool)product.Status,
                        PhotoHeader = directoryToProductPhtoto + product.PhotoHeader,
                        PhotosOfProduct = photosOfProduct,
                        CategoryName = categoryName,
                        DateAdded = product.Created.ToString("MM.dd.yyyy - HH:mm"),
                        DateUpdated = product.Updated.ToString("MM.dd.yyyy - HH:mm")
                    });
                }
            }

            if (productsViewModel.Count > 0)
            {
                switch (sortBy)
                {
                    case "1":
                        productsViewModel = productsViewModel.OrderBy(o => o.DateAdded).ToList(); break;
                    case "-1":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.DateAdded).ToList(); break;
                    case "2":
                        productsViewModel = productsViewModel.OrderBy(o => o.Specials == true ? o.PromotionPrice : o.Price).ToList(); break;
                    case "-2":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.Specials == true ? o.PromotionPrice : o.Price).ToList(); break;
                    case "3":
                        productsViewModel = productsViewModel.OrderBy(o => o.Name).ToList(); break;
                    case "-3":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.Name).ToList(); break;
                    case "4":
                        productsViewModel = productsViewModel.OrderBy(o => o.PromotionPercent).ToList(); break;
                    case "-4":
                        productsViewModel = productsViewModel.OrderByDescending(o => o.PromotionPercent).ToList(); break;
                }
            }

            return productsViewModel;
        }

        public List<CommentVm> AddCommentToProduct(int productId, string userId, string text)
        {
            var commentsVm = new List<CommentVm>();

            var user = db.Users.Find(userId);
            var currentProduct = db.Products.FirstOrDefault(f => f.Id == productId);

            var comment = new Comment()
            {
                CreatedAt = DateTime.Now,
                Text = text,
                UserId = user.Id
            };

            currentProduct.Comments.Add(comment);
            db.SaveChanges();

            var comments = currentProduct.Comments?.OrderByDescending(o => o.CreatedAt).ToList();

            foreach (var commentItem in comments)
            {
                var commentTemp = MapCommentToCommentVm(commentItem);

                if (user?.Id == commentItem.User.Id)
                {
                    commentTemp.OwnComment = true;
                }

                commentsVm.Add(commentTemp);
            }

            return commentsVm;
        }

        public List<CommentVm> AddAdminCommentToProduct(int productId, string userId, string text)
        {
            var commentsVm = new List<CommentVm>();

            var user = db.Users.Find(userId);
            var currentProduct = db.Products.FirstOrDefault(f => f.Id == productId);

            var comment = new Comment()
            {
                CreatedAt = DateTime.Now,
                Text = text,
                UserId = user.Id
            };

            currentProduct.Comments.Add(comment);
            db.SaveChanges();

            var comments = currentProduct.Comments?.OrderByDescending(o => o.CreatedAt).ToList();

            foreach (var commentItem in comments)
            {
                var commentTemp = MapCommentToCommentVm(commentItem);
                commentTemp.OwnComment = true;
                commentsVm.Add(commentTemp);
            }

            return commentsVm;
        }

        public RatingVm AddRatingToProduct(int productId, string userId, int rateNumber)
        {
            var user = db.Users.Find(userId);
            var currentProduct = db.Products.FirstOrDefault(f => f.Id == productId);

            var rating = new Rating()
            {
                CreatedAt = DateTime.Now,
                Number = rateNumber == 0 ? 5 : rateNumber,
                UserId = user.Id
            };

            currentProduct.Ratings.Add(rating);
            db.SaveChanges();

            var ratingsGroup = currentProduct.Ratings.OrderBy(o => o.Number).GroupBy(g => g.Number)?.ToList();

            var ratingVm = new RatingVm();

            var sumRatings = 0;

            currentProduct.Ratings?.ToList().ForEach(f => sumRatings += f.Number);

            ratingVm.OwnRating = rating.Number;
            ratingVm.Average = Math.Round((decimal)sumRatings / currentProduct.Ratings.Count, 1).ToString();
            ratingVm.AllReviews = currentProduct.Ratings.Count.ToString();

            foreach (var ratingGroup in ratingsGroup)
            {
                var ratingNumTemp = ratingGroup.FirstOrDefault();
                var starPercent = Math.Round(((decimal)ratingGroup.Count() / (decimal)currentProduct.Ratings.Count) * 100M, 2);

                switch (ratingNumTemp.Number)
                {
                    case 1: { ratingVm.Star1Percent = starPercent.ToString(); ratingVm.Star1People = ratingGroup.Count().ToString(); break; }
                    case 2: { ratingVm.Star2Percent = starPercent.ToString(); ratingVm.Star2People = ratingGroup.Count().ToString(); break; }
                    case 3: { ratingVm.Star3Percent = starPercent.ToString(); ratingVm.Star3People = ratingGroup.Count().ToString(); break; }
                    case 4: { ratingVm.Star4Percent = starPercent.ToString(); ratingVm.Star4People = ratingGroup.Count().ToString(); break; }
                    case 5: { ratingVm.Star5Percent = starPercent.ToString(); ratingVm.Star5People = ratingGroup.Count().ToString(); break; }
                }
            }

            return ratingVm;
        }

        public List<CommentVm> RemoveCommentToProduct(int productId, string userId, int commentId)
        {
            var commentsVm = new List<CommentVm>();

            var user = db.Users.Find(userId);
            var currentProduct = db.Products.FirstOrDefault(f => f.Id == productId);
            var currentComment = db.Comments.FirstOrDefault(f => f.Id == commentId);

            if (currentComment.User.Id == user.Id)
            {
                db.Comments.Remove(currentComment);
                db.SaveChanges();
            }

            var comments = currentProduct.Comments?.OrderByDescending(o => o.CreatedAt).ToList();

            foreach (var commentItem in comments)
            {
                var commentTemp = MapCommentToCommentVm(commentItem);

                if (user?.Id == commentItem.User.Id)
                {
                    commentTemp.OwnComment = true;
                }

                commentsVm.Add(commentTemp);
            }

            return commentsVm;
        }

        public List<CommentVm> RemoveAdminCommentToProduct(int productId, int commentId)
        {
            var commentsVm = new List<CommentVm>();

            var currentProduct = db.Products.FirstOrDefault(f => f.Id == productId);
            var currentComment = db.Comments.FirstOrDefault(f => f.Id == commentId);


            db.Comments.Remove(currentComment);
            db.SaveChanges();

            var comments = currentProduct.Comments?.OrderByDescending(o => o.CreatedAt).ToList();

            foreach (var commentItem in comments)
            {
                var commentTemp = MapCommentToCommentVm(commentItem);
                commentTemp.OwnComment = true;
                commentsVm.Add(commentTemp);
            }

            return commentsVm;
        }

        public string CreateNewProduct(string productId, string productName, string productBgName, string productMainImgName, List<string> productBodyImgList,
        decimal productPrice, int productCategory, string productCategoryText, string productCategoryBgText, string productCategoryImg, bool productSpecials,
        decimal productPromotionPercent, bool productStatus, bool categoryGender, bool categoryStatus, List<double> productSizeList, List<int> productQuantityList,
        string description, string bgDescription, string userName)
        {
            if (productId == null)
            {
                productId = "0";
            }

            int? productIdInt = null;
            try
            {
                productIdInt = int.Parse(productId);
            }
            catch (Exception)
            {
                // ignored
            }

            var productToEdit = db.Products.FirstOrDefault(w => w.Id == productIdInt);
            UserModel currentUser = db.Users.FirstOrDefault(u => u.Email == userName);

            if (productToEdit != null)
            {
                if (productToEdit.Deleted == false)
                {
                    if (productCategory == 0)
                    {

                        Category newCategory = new Category()
                        {
                            Name = productCategoryText,
                            BgName = productCategoryBgText,
                            Photo = productCategoryImg,
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

                        productToEdit.Name = productName;
                        productToEdit.BgName = productBgName;
                        productToEdit.Price = Math.Round(productPrice, 2);
                        productToEdit.CategoryId = newCategory.Id;
                        productToEdit.Description = description;
                        productToEdit.BgDescription = bgDescription;

                        productToEdit.PromotionPercent = productSpecials ? Math.Round(productPromotionPercent, 2) : 0;

                        if (productMainImgName != null)
                        {
                            productToEdit.PhotoHeader = productMainImgName;
                        }

                        productToEdit.Specials = productSpecials;
                        productToEdit.Updated = DateTime.Now;
                        productToEdit.UpdatedBy = currentUser.Id;
                        productToEdit.Status = productStatus;

                        foreach (var img in productBodyImgList)
                        {
                            PhotoOfProduct newPhotoOfProduct = new PhotoOfProduct()
                            {
                                Photo = img,
                                ProductId = productToEdit.Id
                            };

                            db.PhotoOfProducts.Add(newPhotoOfProduct);
                        }

                        if (productSizeList.Count > 0)
                        {
                            var sizeOfProducts = db.SizeOfProducts.Where(w => w.ProductId == productToEdit.Id);

                            db.SizeOfProducts.RemoveRange(sizeOfProducts);

                            int index = 0;
                            foreach (var size in productSizeList)
                            {
                                SizeOfProduct newSizeOfProduct = new SizeOfProduct()
                                {
                                    ProductId = productToEdit.Id,
                                    Size = Convert.ToInt32(size),
                                    Quantity = productQuantityList[index]
                                };

                                db.SizeOfProducts.Add(newSizeOfProduct);
                                index++;
                            }
                        }

                        db.SaveChanges();
                    }
                    else
                    {
                        Category category = db.Categories.FirstOrDefault(w => w.Id == productCategory);

                        productToEdit.Name = productName;
                        productToEdit.BgName = productBgName;
                        productToEdit.Price = Math.Round(productPrice, 2);
                        productToEdit.CategoryId = category.Id;
                        productToEdit.Description = description;
                        productToEdit.BgDescription = bgDescription;

                        productToEdit.PromotionPercent = productSpecials ? Math.Round(productPromotionPercent, 2) : 0;

                        if (productMainImgName != null)
                        {
                            productToEdit.PhotoHeader = productMainImgName;
                        }

                        productToEdit.Specials = productSpecials;
                        productToEdit.Updated = DateTime.Now;
                        productToEdit.UpdatedBy = currentUser.Id;
                        productToEdit.Status = productStatus;

                        foreach (var img in productBodyImgList)
                        {
                            PhotoOfProduct newPhotoOfProduct = new PhotoOfProduct()
                            {
                                Photo = img,
                                ProductId = productToEdit.Id
                            };

                            db.PhotoOfProducts.Add(newPhotoOfProduct);
                        }

                        if (productSizeList.Count > 0)
                        {
                            var sizeOfProducts = productToEdit.SizeOfProducts;

                            db.SizeOfProducts.RemoveRange(sizeOfProducts);

                            int index = 0;
                            foreach (var size in productSizeList)
                            {
                                SizeOfProduct newSizeOfProduct = new SizeOfProduct()
                                {
                                    ProductId = productToEdit.Id,
                                    Size = Convert.ToInt32(size),
                                    Quantity = productQuantityList[index]
                                };

                                index++;

                                db.SizeOfProducts.Add(newSizeOfProduct);
                            }
                        }

                        db.SaveChanges();
                    }

                    return Resources.Language.MessageProductWasChanged;
                }
                else
                {
                    return Resources.Language.MessageProductBeenDeleted;
                }
            }
            else
            {
                if (productCategory == 0)
                {

                    Category newCategory = new Category()
                    {
                        Name = productCategoryText,
                        BgName = productCategoryBgText,
                        Photo = productCategoryImg,
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

                    Product newProduct = new Product
                    {
                        Name = productName,
                        BgName = productBgName,
                        Price = Math.Round(productPrice, 2),
                        CategoryId = newCategory.Id,
                        Description = description,
                        BgDescription = bgDescription,
                        PromotionPercent =
                                                     productSpecials
                                                         ? Math.Round(productPromotionPercent, 2)
                                                         : 0,
                        PhotoHeader = productMainImgName,
                        Specials = productSpecials,
                        Created = DateTime.Now,
                        CreatedBy = currentUser.Id,
                        Updated = DateTime.Now,
                        UpdatedBy = currentUser.Id,
                        Status = productStatus
                    };


                    db.Products.Add(newProduct);
                    db.SaveChanges();

                    foreach (var img in productBodyImgList)
                    {
                        PhotoOfProduct newPhotoOfProduct = new PhotoOfProduct()
                        {
                            Photo = img,
                            ProductId = newProduct.Id
                        };

                        db.PhotoOfProducts.Add(newPhotoOfProduct);
                    }

                    db.SaveChanges();

                    int index = 0;
                    foreach (var size in productSizeList)
                    {
                        SizeOfProduct newSizeOfProduct = new SizeOfProduct()
                        {
                            ProductId = newProduct.Id,
                            Size = Convert.ToInt32(size),
                            Quantity = productQuantityList[index]
                        };

                        db.SizeOfProducts.Add(newSizeOfProduct);
                        index++;
                    }

                    db.SaveChanges();
                }
                else
                {
                    Category newCategory = db.Categories.FirstOrDefault(w => w.Id == productCategory);

                    Product newProduct = new Product
                    {
                        Name = productName,
                        BgName = productBgName,
                        Price = Math.Round(productPrice, 2),
                        CategoryId = newCategory.Id,
                        Description = description,
                        BgDescription = bgDescription,
                        PromotionPercent =
                                                     productSpecials
                                                         ? Math.Round(productPromotionPercent, 2)
                                                         : 0,
                        PhotoHeader = productMainImgName,
                        Specials = productSpecials,
                        Created = DateTime.Now,
                        CreatedBy = currentUser.Id,
                        Updated = DateTime.Now,
                        UpdatedBy = currentUser.Id,
                        Status = productStatus
                    };


                    db.Products.Add(newProduct);
                    db.SaveChanges();

                    foreach (var img in productBodyImgList)
                    {
                        PhotoOfProduct newPhotoOfProduct = new PhotoOfProduct()
                        {
                            Photo = img,
                            ProductId = newProduct.Id
                        };

                        db.PhotoOfProducts.Add(newPhotoOfProduct);
                    }

                    db.SaveChanges();

                    int index = 0;
                    foreach (var size in productSizeList)
                    {
                        SizeOfProduct newSizeOfProduct = new SizeOfProduct()
                        {
                            ProductId = newProduct.Id,
                            Size = Convert.ToInt32(size),
                            Quantity = productQuantityList[index]
                        };

                        db.SizeOfProducts.Add(newSizeOfProduct);
                        index++;
                    }

                    db.SaveChanges();
                }

                return Resources.Language.MessageProductWasCreated;
            }
        }
    }
}