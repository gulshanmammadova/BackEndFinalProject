using Final_Project_BackEnd.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_Project_BackEnd.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Final_Project_BackEnd.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Final_Project_BackEnd.ViewModels.BlogCommentViewModel;

namespace Final_Project_BackEnd.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public BlogController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int pageIndex=1)
        {
            IQueryable<Blog> blogs =  _context.Blogs.Where(p => p.IsDeleted == false);

            return View(PageNatedList<Blog>.Create(blogs,pageIndex,6));
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (blog == null) return NotFound();


            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            Blog blog = await _context.Blogs
              .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == comment.BlogId);

            BlogCommentVM blogCommentVM = new BlogCommentVM { Blog = blog, Comment = comment };

            if (!ModelState.IsValid) return View("Detail", blogCommentVM);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (blog.Comments != null && blog.Comments.Count() > 0 && blog.Comments.Any(r => r.UserId == appUser.Id))
            {
                ModelState.AddModelError("Name", "Siz Artiq Fikir Bildirmisiz");
                return View("Detail", blogCommentVM);
            }

            comment.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            comment.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }
}
