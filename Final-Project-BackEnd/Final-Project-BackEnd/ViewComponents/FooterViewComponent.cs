using Final_Project_BackEnd.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project_BackEnd.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public FooterViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(IDictionary<string, string> settings)
        {
              return View(await Task.FromResult(settings));
        }
    }
}
