
using Final_Project_BackEnd.DataAccessLayer;
using Final_Project_BackEnd.ViewModels.HomeViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_Project_BackEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM
            {
                Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync(),
                Specials= await _context.Products.Where(p => p.IsDeleted == false && p.IsSpecialSale).ToListAsync(),
                NewArrivals = await _context.Products.Where(p => p.IsDeleted == false && p.IsNewArrival).ToListAsync()

            };

            return View(vm);
        }
    }
}
