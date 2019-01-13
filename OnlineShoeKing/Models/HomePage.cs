
namespace OnlineShoeKing.Models
{
    using System.ComponentModel.DataAnnotations;

    public class HomePage
    {
        [Key]
        public int Id { get; set; }
        public string TextHeader { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
    }
}