using Final_Project_BackEnd.ViewModels.HeaderViewComponentVM;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project_BackEnd.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(HeaderVM headerVM)
        {
            return View(await Task.FromResult(headerVM));
        }
    }
}
