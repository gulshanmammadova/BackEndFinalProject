using Final_Project_BackEnd.DataAccessLayer;
using Final_Project_BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_Project_BackEnd.Controllers
{
    public class InfluencerController : Controller
    {
     
            private readonly AppDbContext _context;

        public InfluencerController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult>  Index()
        {
            IEnumerable<Influencer> influencers = await _context.Influencers.Where(c => c.IsDeleted == false).ToListAsync();

            return View(influencers);
        }
    }
}
