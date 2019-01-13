
namespace OnlineShoeKing.Models
{
    public class CommentVm
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public bool OwnComment { get; set; }
        public string FullName { get; set; }
        public string CreatedAt { get; set; }
        public string Text { get; set; }
    }
}