using Final_Project_BackEnd.DataAccessLayer;
using Final_Project_BackEnd.Models;
using Final_Project_BackEnd.ViewModels;
using Final_Project_BackEnd.ViewModels.HomeViewModel;
using Final_Project_BackEnd.ViewModels.ProductViewsModels;
using Final_Project_BackEnd.ViewModels.ShopViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Final_Project_BackEnd.Controllers
{
    public class ShopController : Controller
    {

        private readonly AppDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public ShopController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int? categoryId, int pageIndex = 1)
        {
            IQueryable<Product> products = _context.Products.Where(p => !p.IsDeleted);
            ShopVM shopVm = new()
            {
                Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync(),
                Products =  PageNatedList<Product>.Create(products.Where(p => (categoryId == null || p.CategoryId == categoryId) && !p.IsDeleted), pageIndex, 6)
            };

            return View(shopVm);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            ProductReviewVM productReviewVM = new ProductReviewVM
            {
                Product = product,
                Review = new Review { ProductId = id },
            };
            return View(productReviewVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddReviuw(Review review)
        {
            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == review.ProductId);

            ProductReviewVM productReviewVM = new ProductReviewVM { Product = product, Review = review };

            if (!ModelState.IsValid) return View("Detail", productReviewVM);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (product.Reviews != null && product.Reviews.Count() > 0 && product.Reviews.Any(r => r.UserId == appUser.Id))
            {
                ModelState.AddModelError("Name", "Siz Artiq Fikir Bildirmisiz");
                return View("Detail", productReviewVM);
            }

            review.UserId = appUser.Id;
            review.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            review.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail), new { id = product.Id });
        }




    }
}
