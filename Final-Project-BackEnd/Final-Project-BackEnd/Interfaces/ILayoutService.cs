using Final_Project_BackEnd.Models;
using Final_Project_BackEnd.ViewModels.BasketViewModels;

namespace Final_Project_BackEnd.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();

        Task<IEnumerable<BasketVM>> GetBaskets();
    }
}
