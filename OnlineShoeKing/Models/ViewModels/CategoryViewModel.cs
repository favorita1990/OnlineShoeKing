

namespace OnlineShoeKing.Models.ViewModels
{
    using System.Collections.Generic;

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string BgName { get; set; }
        public string Photo { get; set; }
        public int? QuantityProduct { get; set; }
        public bool? Status { get; set; }
        public string Created { get; set; }
        public bool? Gender { get; set; }
        public string GenderName { get; set; }
        public string CreatedBy { get; set; }
        public string Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string Deleted { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}