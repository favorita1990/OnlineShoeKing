
namespace OnlineShoeKing.Models
{
    using System;

    public class CartViewModel
    {
        public string CartId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public decimal Total { get; set; }
        public ProductVM ProductInCart { get; set; }
    }

    public class ProductVM
    {
        public string Name { get; set; }
        public string BgName { get; set; }
        public decimal? Price { get; set; }
        public SizeOfProductVM SizeOfProduct { get; set; }
        public bool? Status { get; set; }
        public string Photo { get; set; }
        public DateTime Time { get; set; }
        public bool? Specials { get; set; }
        public int PromotionPercent { get; set; }
        public decimal PromotionPrice { get; set; }
        public string Description { get; set; }
        public string BgDescription { get; set; }
        public string CategoryName { get; set; }
        public string CategoryBgName { get; set; }
    }

    public class SizeOfProductVM
    {
        public int SizeId { get; set; }
        public int Size { get; set; }
    }
}