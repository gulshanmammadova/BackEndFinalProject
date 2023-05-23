using Final_Project_BackEnd.DataAccessLayer;
using Final_Project_BackEnd.Extentions;
using Final_Project_BackEnd.Helpers;
using Final_Project_BackEnd.Models;
using Final_Project_BackEnd.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing.Drawing2D;

namespace Final_Project_BackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int pageindex=1)
        {
            IQueryable<Category> categories = _context.Categories
                .Include(c => c.Products)
                .Where(c => c.IsDeleted == false );

            return View(PageNatedList<Category>.Create(categories, pageindex, 5));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                 .Include(a => a.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            return View(category);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Products.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Products.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow.AddHours(4);
            category.DeletedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false ).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower().Contains(category.Name.Trim().ToLower())))
            {
                ModelState.AddModelError("Name", $"Bu Adda {category.Name.Trim()} Categoriya Movcuddur");
                return View(category);
            }

            
                if (category.File == null)
                {
                    ModelState.AddModelError("File", "Sekil Mecburidi");
                    return View(category);
                }

                if (!category.File.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("File", "Fayl Tipi Duz Deyil");
                    return View(category);
                }

                if (!category.File.CheckFileLength(300))
                {
                    ModelState.AddModelError("File", "Fayl Olcusu Maksimum 30 kb Ola Biler");
                    return View(category);
                }

                category.Image = await category.File.CraeteFileAsync(_env, "assets", "img","category");

               
            

            category.Name = category.Name.Trim();
            category.CreatedAt = DateTime.UtcNow.AddHours(4);
            category.CreatedBy = "System";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false ).ToListAsync();

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false ).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            if (id == null) return BadRequest();

            if (id != category.Id) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (dbCategory == null) return NotFound();

            
                 if (category.File != null)
                    {
                        if (!category.File.CheckFileContentType("image/jpeg"))
                        {
                            ModelState.AddModelError("File", "Fayl Tipi Duz Deyil");
                            return View(category);
                        }

                        if (!category.File.CheckFileLength(300))
                        {
                            ModelState.AddModelError("File", "Fayl Olcusu Maksimum 30 kb Ola Biler");
                            return View(category);
                        }

                        FileHelper.DeleteFile(dbCategory.Image, _env, "assets", "img" ,"category");

                        dbCategory.Image = await category.File.CraeteFileAsync(_env, "assets", "img" ,"category");
                    }
                
            dbCategory.Name = category.Name.Trim();
            dbCategory.UpdatedBy = "System";
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
