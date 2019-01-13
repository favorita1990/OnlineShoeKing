

namespace OnlineShoeKing.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Context;
    using Models;

    public class ShoppingCart
    {
        private readonly DBContext db = new DBContext();
        private readonly UserRepository userRepository = new UserRepository();
        private readonly ProductRepository productRepository = new ProductRepository();
        private readonly string getProductDirectory = Resources.Pictures.PhotoPathProducts;

        public string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public List<CartViewModel> AddToCart(int productId, int sizeId)
        {
            // Retrieve the product from the database.           
            ShoppingCartId = GetCartId();

            var cartItem = db.Carts.FirstOrDefault(
                c => c.CartId == ShoppingCartId);

            var cartSizeItem = db.SizeOfProducts.FirstOrDefault(f => f.Id == sizeId && f.ProductId == productId);

            decimal total = 0;

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists.                 
                cartItem = new Cart
                {
                    ProductId = productId,
                    CartId = ShoppingCartId,
                    Quantity = 1,
                    DateCreated = DateTime.Now,
                    SizeId = cartSizeItem.Id
                };

                db.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart,                  
                // then add one to the quantity.  
                var cartItem2 = new Cart
                                    {
                                        ProductId = productId,
                                        CartId = ShoppingCartId,
                                        DateCreated = DateTime.Now,
                                        SizeId = cartSizeItem.Id,
                                        Quantity = this.db.Carts.Count(w => w.CartId == this.ShoppingCartId) + 1
                                    };

                db.Carts.Add(cartItem2);
            }

            db.SaveChanges();

            var cart = db.Carts.Where(w => w.CartId == ShoppingCartId).ToList();

            List<CartViewModel> listCartViewModel = new List<CartViewModel>();

            foreach (var cartItem2 in cart)
            {
                var productItem = db.Products.SingleOrDefault(p => p.Id == cartItem2.ProductId);
                var category = db.Categories.SingleOrDefault(c => c.Id == productItem.CategoryId);
                var categoryName = category.Name;
                var categoryBgName = category.BgName;
                var promotionPrice = (decimal)productItem.Price;

                var currentSize = productItem.SizeOfProducts.FirstOrDefault(f => f.Id == cartItem2.SizeId);

                SizeOfProductVM size = new SizeOfProductVM()
                {
                    SizeId = currentSize.Id,
                    Size = currentSize.Size
                };

                if (productItem.Specials == true)
                {
                    promotionPrice = (decimal)productItem.Price * (1.0M - (productItem.PromotionPercent / 100));

                    total += Math.Round(promotionPrice, 2);

                    var cartViewModel = new CartViewModel()
                    {
                        CartId = cartItem2.CartId,
                        ProductId = cartItem2.ProductId,
                        Quantity = cartItem2.Quantity,
                        Total = total,
                        ProductInCart = new ProductVM()
                        {
                            Name = productItem.Name,
                            BgName = productItem.BgName,
                            Price = productItem.Price,
                            SizeOfProduct = size,
                            Status = productItem.Status,
                            Photo = getProductDirectory + productItem.PhotoHeader,
                            Time = productItem.Updated,
                            Specials = productItem.Specials,
                            PromotionPercent = (int)productItem.PromotionPercent,
                            PromotionPrice = Math.Round(promotionPrice, 2),
                            Description = productItem.Description,
                            BgDescription = productItem.BgDescription,
                            CategoryName = categoryName,
                            CategoryBgName = categoryBgName
                        }
                    };

                    listCartViewModel.Add(cartViewModel);
                }
                else
                {
                    total += Math.Round((decimal)productItem.Price, 2);

                    var cartViewModel = new CartViewModel()
                    {
                        CartId = cartItem2.CartId,
                        ProductId = cartItem2.ProductId,
                        Quantity = cartItem2.Quantity,
                        Total = total,
                        ProductInCart = new ProductVM()
                        {
                            Name = productItem.Name,
                            BgName = productItem.BgName,
                            Price = productItem.Price,
                            SizeOfProduct = size,
                            Status = productItem.Status,
                            Photo = getProductDirectory + productItem.PhotoHeader,
                            Time = productItem.Updated,
                            Specials = productItem.Specials,
                            PromotionPercent = 0,
                            PromotionPrice = productItem.PromotionPercent,
                            Description = productItem.Description,
                            BgDescription = productItem.BgDescription,
                            CategoryName = categoryName,
                            CategoryBgName = categoryBgName
                        }
                    };

                    listCartViewModel.Add(cartViewModel);
                }
            }
            
            return listCartViewModel;
        }

        public List<CartViewModel> AddToCartProducts(int productId, int sizeValue)
        {
            // Retrieve the product from the database.           
            ShoppingCartId = GetCartId();

            var cartItem = db.Carts.FirstOrDefault(
                c => c.CartId == ShoppingCartId);

            var cartSizeItem = db.SizeOfProducts.FirstOrDefault(f => (f.Size == sizeValue) && (f.ProductId == productId));

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists.                 
                cartItem = new Cart
                {
                    ProductId = productId,
                    CartId = ShoppingCartId,
                    Quantity = 1,
                    DateCreated = DateTime.Now,
                    SizeId = cartSizeItem.Id
                };

                db.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart,                  
                // then add one to the quantity.  
                var cartItem2 = new Cart
                                    {
                                        ProductId = productId,
                                        CartId = ShoppingCartId,
                                        DateCreated = DateTime.Now,
                                        SizeId = cartSizeItem.Id,
                                        Quantity = this.db.Carts.Count(w => w.CartId == this.ShoppingCartId) + 1
                                    };

                db.Carts.Add(cartItem2);
            }

            db.SaveChanges();

            decimal total = 0;

            var cart = db.Carts.Where(w => w.CartId == ShoppingCartId).ToList();

            List<CartViewModel> listCartViewModel = new List<CartViewModel>();

            foreach (var cartItem2 in cart)
            {
                var productItem = db.Products.SingleOrDefault(p => p.Id == cartItem2.ProductId);
                string categoryName = db.Categories.SingleOrDefault(c => c.Id == productItem.CategoryId).Name;

                decimal promotionPrice = (decimal)productItem.Price;

                var currentSize = productItem.SizeOfProducts.FirstOrDefault(f => f.Id == cartItem2.SizeId);

                SizeOfProductVM size = new SizeOfProductVM()
                {
                    SizeId = currentSize.Id,
                    Size = currentSize.Size
                };

                if (productItem.Specials == true)
                {
                    promotionPrice = (decimal)productItem.Price * (1.0M - (productItem.PromotionPercent / 100));

                    total += Math.Round(promotionPrice, 2);

                    var cartViewModel = new CartViewModel()
                    {
                        CartId = cartItem2.CartId,
                        ProductId = cartItem2.ProductId,
                        Quantity = cartItem2.Quantity,
                        Total = total,
                        ProductInCart = new ProductVM()
                        {
                            Name = productItem.Name,
                            Price = productItem.Price,
                            SizeOfProduct = size,
                            Status = productItem.Status,
                            Photo = getProductDirectory + productItem.PhotoHeader,
                            Time = productItem.Updated,
                            Specials = productItem.Specials,
                            PromotionPercent = (int)productItem.PromotionPercent,
                            PromotionPrice = Math.Round(promotionPrice, 2),
                            Description = productItem.Description,
                            CategoryName = categoryName,
                        }
                    };

                    listCartViewModel.Add(cartViewModel);
                }
                else
                {
                    total += Math.Round((decimal)productItem.Price, 2);

                    var cartViewModel = new CartViewModel()
                    {
                        CartId = cartItem2.CartId,
                        ProductId = cartItem2.ProductId,
                        Quantity = cartItem2.Quantity,
                        Total = total,
                        ProductInCart = new ProductVM()
                        {
                            Name = productItem.Name,
                            Price = productItem.Price,
                            SizeOfProduct = size,
                            Status = productItem.Status,
                            Photo = getProductDirectory + productItem.PhotoHeader,
                            Time = productItem.Updated,
                            Specials = productItem.Specials,
                            PromotionPercent = 0,
                            PromotionPrice = productItem.PromotionPercent,
                            Description = productItem.Description,
                            CategoryName = categoryName,
                        }
                    };

                    listCartViewModel.Add(cartViewModel);
                }
            }

            return listCartViewModel;
        }

        public decimal RemoveFromCart(int productId)
        {
            ShoppingCartId = GetCartId();

            var cartItem = db.Carts.FirstOrDefault(
                c => c.CartId == ShoppingCartId &&
                c.ProductId == productId);

            if (cartItem != null) this.db.Carts.Remove(cartItem);
            db.SaveChanges();

            var cartItems = db.Carts.Where(w => w.CartId == ShoppingCartId).ToList();

            decimal total = 0;

            foreach (var item in cartItems)
            {
                var productItem = db.Products.SingleOrDefault(p => p.Id == item.ProductId);

                total += (decimal)productRepository.productPromo(productItem);
            }

            return total;
        }

        public int Checkout()
        {
            var cartItems = GetCartItems();
            var userNameIsAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            var currentUserName = HttpContext.Current.User.Identity.Name;

            if (cartItems.Count <= 0)
            {
                return -3;
            }
            else if(!userNameIsAuthenticated)
            {
                return -2;
            }
            else
            {
                return 1;
            }
        }

        public int CartOrder(string city, string address, string postOfiice, string phoneNumber)
        {
            var cartItems = GetCartItems();
            var userNameIsAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            var currentUserName = HttpContext.Current.User.Identity.Name;

            if (cartItems.Count <= 0)
            {
                return -3;
            }
            else if (!userNameIsAuthenticated)
            {
                return -2;
            }
            else
            {
                decimal? orderTotal = 0;
                var orderStatusId = db.OrderStatuses.First(f => f.Name == "waiting").Id;

                var order = new Order
                                  {
                                      Created = DateTime.Now,
                                      Updated = DateTime.Now,
                                      UserId = this.userRepository.UserId(currentUserName),
                                      City = city,
                                      Address = address,
                                      PostOffice = postOfiice,
                                      PhoneNumber = phoneNumber,
                                      OrderStatusId = orderStatusId,
                                      Deleted = false
                                  };

                db.Orders.Add(order);
                db.SaveChanges();

                var quantityCheck = 0;

                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                                                  {
                                                      OrderId = order.Id,
                                                      ProductId = item.product.Id,
                                                      Status = item.product.Status,
                                                      Specials = item.product.Specials,
                                                      Price = this.productRepository.productPromo(
                                                          item.product)
                                                  };

                    orderTotal += orderDetail.Price;
                    orderDetail.SizeId = item.SizeId;
                    orderDetail.Quantity = productRepository.ProductQuantitySize(item.SizeId);

                    if (orderDetail.Quantity == -1)
                    {
                        quantityCheck = -1;
                    }

                    db.OrderDetails.Add(orderDetail);
                }
                order.Total = orderTotal;

                if (quantityCheck != 0)
                {
                    db.Orders.Remove(order);
                    return -1;
                }

                productRepository.UpdateSizeOfProductQantity(cartItems);
                db.SaveChanges();

                EmptyCart();
                return 1;
            }
        }

        public List<CartViewModel> CheckoutProblem()
        {
            var cartItems = GetCartItems();

            List<CartViewModel> listCartViewModel = new List<CartViewModel>();

            var language = Resources.Language.BG == "БГ" ? true : false;

            foreach (var cartItem in cartItems)
            {

                var sizeCart = db.SizeOfProducts.Find(cartItem.SizeId);
                sizeCart.Quantity--;
                if (sizeCart.Quantity < 0)
                {
                    var productItem = db.Products.SingleOrDefault(p => p.Id == cartItem.ProductId);
                    string categoryName = language ? db.Categories.FirstOrDefault(c => c.Id == productItem.CategoryId).BgName :
                        db.Categories.FirstOrDefault(c => c.Id == productItem.CategoryId).Name;

                    SizeOfProductVM size = new SizeOfProductVM()
                    {
                        SizeId = sizeCart.Id,
                        Size = sizeCart.Size
                    };


                    var cartViewModel = new CartViewModel()
                    {
                        CartId = cartItem.CartId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        ProductInCart = new ProductVM()
                        {
                            Name = language ? productItem.BgName : productItem.Name,
                            Price = productItem.Price,
                            SizeOfProduct = size,
                            Status = productItem.Status,
                            Photo = getProductDirectory + productItem.PhotoHeader,
                            Time = productItem.Updated,
                            Specials = productItem.Specials,
                            PromotionPercent = (int)productItem.PromotionPercent,
                            PromotionPrice = (decimal)productRepository.productPromo(productItem),
                            Description = productItem.Description,
                            CategoryName = categoryName,
                        }
                    };

                    listCartViewModel.Add(cartViewModel);
                }
            }

            return listCartViewModel;
        }

        public List<CartViewModel> GetCartProducts()
        {
            var cartItems = GetCartItems();

            List<CartViewModel> listCartViewModel = null;

            var language = Resources.Language.BG == "БГ" ? true : false;

            decimal total = 0;

            if (cartItems != null)
            {
                listCartViewModel = new List<CartViewModel>();

                foreach (var cartItem in cartItems)
                {
                    var productItem = db.Products.SingleOrDefault(p => p.Id == cartItem.ProductId);
                    var categoryName = language ? db.Categories.FirstOrDefault(c => c.Id == productItem.CategoryId).BgName : 
                        db.Categories.FirstOrDefault(c => c.Id == productItem.CategoryId).Name;

                    var promotionPrice = (decimal)productItem.Price;

                    var sizeCart = db.SizeOfProducts.Find(cartItem.SizeId);
                    var size = new SizeOfProductVM()
                    {
                        SizeId = sizeCart.Id,
                        Size = sizeCart.Size
                    };

                    total += (decimal)productRepository.productPromo(productItem);

                    var cartViewModel = new CartViewModel()
                    {
                        CartId = cartItem.CartId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Total = total,
                        ProductInCart = new ProductVM()
                        {
                            Name = language ? productItem.BgName : productItem.Name,
                            Price = productItem.Price,
                            SizeOfProduct = size,
                            Status = productItem.Status,
                            Photo = getProductDirectory + productItem.PhotoHeader,
                            Time = productItem.Updated,
                            Specials = productItem.Specials,
                            PromotionPercent = (int)productItem.PromotionPercent,
                            PromotionPrice = (decimal)productRepository.productPromo(productItem),
                            Description = productItem.Description,
                            CategoryName = categoryName,
                        }
                    };

                    listCartViewModel.Add(cartViewModel);
                }
            }

            return listCartViewModel;
        }

        public string GetCartId()
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class.     
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return HttpContext.Current.Session[CartSessionKey].ToString();
        }

        public List<Cart> GetCartItems()
        {
            ShoppingCartId = GetCartId();

            return db.Carts.Where(c => c.CartId == ShoppingCartId).ToList();
        }

        public void RemoveItem(string removeCartID, int removeProductID)
        {
            using (var db = new DBContext())
            {
                try
                {
                    var myItem = (from c in db.Carts where c.CartId == removeCartID && c.ProductId == removeProductID select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        // Remove Item.
                        db.Carts.Remove(myItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("ERROR: Unable to Remove Cart Item - " + exp.Message, exp);
                }
            }
        }

        public void EmptyCart()
        {
            ShoppingCartId = GetCartId();
            var cartItems = db.Carts.Where(
                c => c.CartId == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                db.Carts.Remove(cartItem);
            }
            // Save changes.             
            db.SaveChanges();
        }

        public int GetCount()
        {
            ShoppingCartId = GetCartId();

            // Get the count of each item in the cart and sum them up          
            int? count = (from cartItems in db.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Quantity).Sum();
            // Return 0 if all entries are null         
            return count ?? 0;
        }

        //public void AddToCart(Product product)
        //{
        //    var cartItem = myShopDBEntities.Carts.SingleOrDefault(
        //        c => c.ProductId == product.Id);

        //    if (cartItem == null)
        //    {
        //        cartItem = new Cart
        //        {
        //            ProductId = product.Id,
        //            Count = 1,
        //        };
        //        myShopDBEntities.Carts.Add(cartItem);
        //    }
        //    else
        //    {
        //        cartItem.Count++;
        //    }
        //    myShopDBEntities.SaveChanges();
        //}
        //public void RemoveFromCart(int id)
        //{
        //    var cartItem = myShopDBEntities.Carts.FirstOrDefault(
        //        cart => cart.Id == id);

        //    if (cartItem != null)
        //    {
        //        if (cartItem.Count > 1)
        //        {
        //            cartItem.Count--;
        //        }
        //        else
        //        {
        //            myShopDBEntities.Carts.Remove(cartItem);
        //        }
        //        myShopDBEntities.SaveChanges();
        //    }
        //}

        //public void EmptyCart()
        //        {
        //            var cartItems = myShopDBEntities.Carts.ToList();

        //            foreach (var cartItem in cartItems)
        //            {
        //                myShopDBEntities.Carts.Remove(cartItem);
        //            }
        //            myShopDBEntities.SaveChanges();
        //        }

        //public List<Cart> GetCartItems()
        //{
        //    return myShopDBEntities.Carts.ToList();
        //}
        //public int GetCount()
        //{
        //    int? count = (from cartItems in myShopDBEntities.Carts
        //                  select (int?)cartItems.Count).Sum();
        //    return count ?? 0;
        //}
        //public decimal GetTotal()
        //{
        //    var iProductRepository = new ProductRepository();
        //    var cartItems = myShopDBEntities.Carts.ToList();
        //    decimal? total = 0;

        //    foreach (var item in cartItems)
        //    {
        //        if (item.product.Status == true)
        //        {
        //            decimal? promo = iProductRepository.productPromo(item.product);
        //            total += (item.Count * promo);

        //        }
        //        else
        //        {
        //            total += (item.Count * item.product.Price);
        //        }
        //    }

        //    return total ?? decimal.Zero;
        //}
        //public void CreateOrder(string currentName)
        //{
        //    decimal? orderTotal = 0;
        //    var iUserRepository = new UserRepository();
        //    var iProductRepository = new ProductRepository();

        //    var cartItems = GetCartItems();

        //    Order order = new Order();
        //    order.CreationDate = DateTime.Now;
        //    order.Name = "New Order";
        //    order.UserName = currentName;
        //    order.UserId = iUserRepository.UserId(currentName);
        //    List<int> order1 = myShopDBEntities.Orders.Select(e => e.Id).ToList();
        //    order.Id = order1.Count;
        //    myShopDBEntities.Orders.Add(order);
        //    int orderId = order.Id;
        //    foreach (var item in cartItems)
        //    {
        //        OrderDetail orderDetail = new OrderDetail();
        //        orderDetail.OrderId = orderId;
        //        orderDetail.ProductId = item.product.Id;
        //        orderDetail.Status = item.product.Status;
        //        orderDetail.Specials = item.product.Specials;
        //        if (item.product.Status == true)
        //        {
        //            orderDetail.Price = iProductRepository.productPromo(item.product);
        //            orderTotal += (item.Count * orderDetail.Price);
        //        }
        //        else
        //        {
        //            orderDetail.Price = item.product.Price;
        //            orderTotal += (item.Count * item.product.Price);
        //        }
        //        orderDetail.Quantity = item.Count;

        //        myShopDBEntities.OrderDetails.Add(orderDetail);

        //    }
        //    order.Total = orderTotal;

        //    myShopDBEntities.SaveChanges();

        //    EmptyCart();
        //}
    }
}