

namespace OnlineShoeKing.Models
{
    public class CollectionsViewModel
    {
        public int CollectionId { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public int ProductCount { get; set; }
        public string ProductLastAdded { get; set; }
    }

    public class HomePageViewModel
    {
        public int Id { get; set; }
        public string TextHeader { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
    }

    public class HomeCollectionsViewModel
    {
        public int CollectionId { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
    }

    public class HomeNewArrivalsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BgName { get; set; }
        public decimal Price { get; set; }
        public SizeOfHomeProduct SizeOfProduct { get; set; }
        public bool Status { get; set; }
        public string Photo { get; set; }
        public bool? Specials { get; set; }
        public int PromotionPercent { get; set; }
        public decimal PromotionPrice { get; set; }
        public string Description { get; set; }
        public string BgDescription { get; set; }
        public int ProductId { get; set; }
    }

    public class HomeDiscountsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BgName { get; set; }
        public decimal Price { get; set; }
        public SizeOfHomeProduct SizeOfProduct { get; set; }
        public bool Status { get; set; }
        public string Photo { get; set; }
        public bool? Specials { get; set; }
        public int PromotionPercent { get; set; }
        public decimal PromotionPrice { get; set; }
        public string Description { get; set; }
        public string BgDescription { get; set; }
        public int ProductId { get; set; }
    }

    public class HomeMostBoughtViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BgName { get; set; }
        public decimal Price { get; set; }
        public SizeOfHomeProduct SizeOfProduct { get; set; }
        public bool Status { get; set; }
        public string Photo { get; set; }
        public bool? Specials { get; set; }
        public int PromotionPercent { get; set; }
        public decimal PromotionPrice { get; set; }
        public string Description { get; set; }
        public string BgDescription { get; set; }
        public int ProductId { get; set; }
    }

    public class SizeOfHomeProduct
    {
        public int SizeId { get; set; }
        public int Size { get; set; }
    }
}