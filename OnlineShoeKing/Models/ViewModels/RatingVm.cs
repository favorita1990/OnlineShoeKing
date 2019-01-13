
namespace OnlineShoeKing.Models
{
    public class RatingVm
    {
        public int Id { get; set; }
        public int OwnRating { get; set; }
        public string Star1Percent { get; set; }
        public string Star1People { get; set; }
        public string Star2Percent { get; set; }
        public string Star2People { get; set; }
        public string Star3Percent { get; set; }
        public string Star3People { get; set; }
        public string Star4Percent { get; set; }
        public string Star4People { get; set; }
        public string Star5Percent { get; set; }
        public string Star5People { get; set; }
        public string Average { get; set; }
        public string AllReviews { get; set; }
    }
}