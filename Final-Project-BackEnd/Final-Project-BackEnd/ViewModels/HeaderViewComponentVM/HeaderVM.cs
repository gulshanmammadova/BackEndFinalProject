using Final_Project_BackEnd.Models;
using Final_Project_BackEnd.ViewModels.BasketViewModels;

namespace Final_Project_BackEnd.ViewModels.HeaderViewComponentVM
{
    public class HeaderVM
    {
        public IDictionary<string, string> Settings { get; set; }
        public IEnumerable<BasketVM> BasketVMs { get; set; }
    }
}
