
namespace OnlineShoeKing.Repository
{
    using System.Collections.Generic;
    using OnlineShoeKing.Models;

    public interface ICategoryRepository
    {
        List<Category> findAll();
        Category find(int id);
        List<Product> PromotionProducts(int n = 4);
    }
}
