using Final_Project_BackEnd.Models;

namespace Final_Project_BackEnd.ViewModels.ShopViewModels
{
    public class ShopVM
    {
        public IEnumerable<Category>? Categories { get; set; }
        public PageNatedList<Product>? Products { get; set; }
    }
}
