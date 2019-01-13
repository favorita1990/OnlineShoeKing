
namespace OnlineShoeKing.Repository
{
    using System.Collections.Generic;

    using Models;

    public interface IProductRepository
    {
        List<Product> LatestProducts(int n);
        List<Product> LatestProducts();
        List<Product> FeaturedProducts(int n);
        List<Product> specialProducts();
        List<Product> RalatedProducts(Product product, int n);
        Product find(int id);
        decimal? productPromo(Product product, decimal n = 0.9M);
        decimal? productPromoSaved(Product product, decimal n = 0.9M);
    }
}
