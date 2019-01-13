
namespace OnlineShoeKing.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ProductViewModel
    {
        //[ScaffoldColumn(false)]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string BgName { get; set; }
        public decimal? Price { get; set; }
        public List<SizesOfProduct> SizesOfProduct { get; set; }
        public List<string> PhotosOfProduct { get; set; }
        public bool? Status { get; set; }
        public string PhotoHeader { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public int PurchasedProductsCount { get; set; }
        public string UpdatedBy { get; set; }
        public bool? Specials { get; set; }
        public int PromotionPercent { get; set; }
        public decimal PromotionPrice { get; set; }
        public string Description { get; set; }
        public string BgDescription { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryBgName { get; set; }
        public bool? CategoryGender { get; set; }
        public string CategoryPhoto { get; set; }
        public string Deleted { get; set; }
    }

    public class ProductViewModelGrid
    {
        [ScaffoldColumn(false)]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public bool? Status { get; set; }
        public string PhotoHeader { get; set; }
        [ScaffoldColumn(false)]
        public string DateAdded { get; set; }
        public bool? Specials { get; set; }
        public int PromotionPercent { get; set; }
        public decimal PromotionPrice { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
    }

    public class SizesOfProduct
    {
        public int SizeId { get; set; }
        public int Size { get; set; }
        public int? Quantity { get; set; }
    }

    public class PhotoOfProductVM
    {
        public int PhotoId { get; set; }
        public string Photo { get; set; }
        public int ProductId { get; set; }
    }
}