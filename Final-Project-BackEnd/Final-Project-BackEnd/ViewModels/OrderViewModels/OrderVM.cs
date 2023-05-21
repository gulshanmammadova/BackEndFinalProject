using Final_Project_BackEnd.Models;
using Final_Project_BackEnd.ViewModels.BasketViewModels;

namespace Final_Project_BackEnd.ViewModels.OrderViewModels
{
    public class OrderVM
    {
        public Order? Order { get; set; }
        public IEnumerable<BasketVM> BasketVMs { get; set; }

    }
}
