using Final_Project_BackEnd.DataAccessLayer;
using Final_Project_BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_Project_BackEnd.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppDbContext _context;

        public AboutController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Team> teams = await _context.Teams.Where(p => p.IsDeleted == false).ToListAsync();

            return View(teams);
        }
    }
}
