using Final_Project_BackEnd.Models;

namespace Final_Project_BackEnd.ViewModels.HomeViewModel
{
    public class HomeVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> NewArrivals { get; set; }
        public IEnumerable<Product> Specials { get; set; }
    }
}
