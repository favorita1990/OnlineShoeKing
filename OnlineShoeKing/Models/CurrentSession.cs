using System;
using System.Collections.Generic;
using System.Web;

namespace OnlineShoeKing.Models
{
    public class CurrentSession
    {
        public static List<HttpPostedFileBase> ProductBodyImagesList;

        public static List<Tuple<HttpPostedFileBase, string>> HomeSlideTempImagesList;

        public static List<string> HomeSlideTempImagesReadyList;

        public static List<int> ListProducts;

        public static string currentUserName;

        public static HttpPostedFileBase ProductMainImage
        {
            get
            {
                if (null != HttpContext.Current.Session["productMainImage"])
                    return HttpContext.Current.Session["productMainImage"] as HttpPostedFileBase;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["productMainImage"] = value;
            }
        }

        public static List<HttpPostedFileBase> ProductBodyImages
        {
            get
            {
                return ProductBodyImagesList;
            }
            set
            {
                if(ProductBodyImagesList == null)
                {
                    ProductBodyImagesList = new List<HttpPostedFileBase>();
                }

                if(value != null)
                {
                    ProductBodyImagesList.Add(value[0]);
                }
                else
                {
                    ProductBodyImagesList = null;
                }
                
                
            }
        }

        public static List<Tuple<HttpPostedFileBase, string>> GetSetHomeSlidesTempImages
        {
            get
            {
                return HomeSlideTempImagesList;
            }
            set
            {
                if (HomeSlideTempImagesList == null)
                {
                    HomeSlideTempImagesList = new List<Tuple<HttpPostedFileBase, string>>();
                }

                HomeSlideTempImagesList = value;
            }
        }

        public static string AddHomeSlidesTempImagesFirstTime
        {
            set
            {
                if (HomeSlideTempImagesReadyList == null)
                {
                    HomeSlideTempImagesReadyList = new List<string>();
                }

                if (value != null)
                {
                    HomeSlideTempImagesReadyList.Add(value);
                }
                else
                {
                    HomeSlideTempImagesReadyList = null;
                }
            }
        }

        public static List<string> HomeSlidesTempImagesFirstTime
        {
            get
            {
                return HomeSlideTempImagesReadyList;
            }
            set
            {
                if (HomeSlideTempImagesReadyList == null)
                {
                    HomeSlideTempImagesReadyList = new List<string>();
                }

                HomeSlideTempImagesReadyList = value;
            }
        }

        public static HttpPostedFileBase NewCategoryImage
        {
            get
            {
                if (null != HttpContext.Current.Session["newCategoryImage"])
                    return HttpContext.Current.Session["newCategoryImage"] as HttpPostedFileBase;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["newCategoryImage"] = value;
            }
        }

        public static string ProductAddOrEditProductId
        {
            get
            {
                if (null != HttpContext.Current.Session["productAddOrEditProductId"])
                    return HttpContext.Current.Session["productAddOrEditProductId"] as string;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["productAddOrEditProductId"] = value;
            }
        }

        public static string CategoryAddOrEditCategoryId
        {
            get
            {
                if (null != HttpContext.Current.Session["categoryAddOrEditCategoryId"])
                    return HttpContext.Current.Session["categoryAddOrEditCategoryId"] as string;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["categoryAddOrEditCategoryId"] = value;
            }
        }

        public static List<ProductViewModel> SearchListProducts
        {
            get
            {
                if (null != HttpContext.Current.Session["searchListProducts"])
                    return HttpContext.Current.Session["searchListProducts"] as List<ProductViewModel>;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["searchListProducts"] = value;
            }
        }

        public static List<UserViewModel> SearchListUsers
        {
            get
            {
                if (null != HttpContext.Current.Session["searchListUsers"])
                    return HttpContext.Current.Session["searchListUsers"] as List<UserViewModel>;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["searchListUsers"] = value;
            }
        }

        public static string CurrentUser
        {
            get
            {
                return currentUserName;
            }
            set
            {
                currentUserName = value;
            }
        }

        public static string SortBy
        {
            get
            {
                if (null != HttpContext.Current.Session["sortBy"])
                    return HttpContext.Current.Session["sortBy"] as string;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["sortBy"] = value;
            }
        }

        public static int? ProductId
        {
            get
            {
                if (null != HttpContext.Current.Session["productId"])
                    return HttpContext.Current.Session["productId"] as int?;
                return null;
            }
            set
            {
                HttpContext.Current.Session["productId"] = value;
            }
        }

        public static List<int> GetListProducts
        {
            get
            {
                if (ListProducts == null)
                {
                    ListProducts = new List<int>();
                }

                return ListProducts;
            }
        }

        public static int AddListProducts
        {
            set
            {
                if(ListProducts == null)
                {
                    ListProducts = new List<int>();
                }
                ListProducts.Add(value);
            }
        }
    }
}