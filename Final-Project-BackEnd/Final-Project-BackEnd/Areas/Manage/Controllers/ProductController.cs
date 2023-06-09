﻿using Final_Project_BackEnd.DataAccessLayer;
using Final_Project_BackEnd.Extentions;
using Final_Project_BackEnd.Helpers;
using Final_Project_BackEnd.Models;
using Final_Project_BackEnd.Areas.Manage.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_Project_BackEnd.ViewModels;
using Final_Project_BackEnd.ViewModels;
using Microsoft.AspNetCore.Authorization;



namespace Final_Project_BackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]

    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Product> products = _context.Products.Where(p => p.IsDeleted == false);

            return View(PageNatedList<Product>.Create(products, pageIndex, 3));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories
                .Where(b => b.IsDeleted == false )
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = await _context.Categories
                .Where(b => b.IsDeleted == false )
                .ToListAsync();

             if (!ModelState.IsValid) return View(product);

           

            if (!await _context.Categories.AnyAsync(b => b.IsDeleted == false && b.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", $"Daxil Olunan Category Id {product.CategoryId} Yanlisdir");
                return View(product);
            }

           

            if (product.MainFile != null)
            {
                if (!product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz JPG Olmalidir");
                    return View(product);
                }

                if (!product.MainFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz 300 kb Olmalidir");
                    return View(product);
                }

                product.MainImage = await product.MainFile.CraeteFileAsync(_env, "assets", "img", "product");
            }
            else
            {
                ModelState.AddModelError("MainFile", "Main File Mutleq Daxil Olmalidir");
                return View(product);
            }

          

            if (product.Files == null)
            {
                ModelState.AddModelError("Files", "Sekil Mutleq Secilmelidir");
                return View(product);
            }

            if (product.Files.Count() > 6)
            {
                ModelState.AddModelError("Files", "Maksimum 6 Sekil Yukleye Bilersiniz");
                return View(product);
            }
            AppUser appUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == (User.Identity.Name.ToLowerInvariant()));

            if (product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    if (!file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Yalniz JPG Olmalidir");
                        return View(product);
                    }

                    if (!file.CheckFileLength(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Yalniz 300 kb Olmalidir");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CraeteFileAsync(_env, "assets", "img", "product"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = $"{appUser.Name}{appUser.SurName}"
                    };

                    productImages.Add(productImage);
                }

                product.ProductImages = productImages;

            }

            string code = "";
            code = code + _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId).Name.Substring(0, 2);

            product.Seria = code.ToLower().Trim();
            product.Code = _context.Products.Where(p => p.Seria == product.Seria).OrderByDescending(p => p.Id).FirstOrDefault() != null ?
                _context.Products.Where(p => p.Seria == product.Seria).OrderByDescending(p => p.Id).FirstOrDefault().Code + 1 : 1;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();

            return View(product);
        }


        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            ViewBag.Categories = await _context.Categories
                .Where(b => b.IsDeleted == false)
                .ToListAsync();

             return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Categories = await _context.Categories
                .Where(b => b.IsDeleted == false )
                .ToListAsync();

             if (!ModelState.IsValid)
            {
                return View();
            }

            if (id == null) return BadRequest();

            if (id != product.Id) return BadRequest();

            Product dbProduct = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
           if (dbProduct == null) return NotFound();

            int canUpload = 6 - dbProduct.ProductImages.Count();

            if (product.Files != null && canUpload < product.Files.Count())
            {
                ModelState.AddModelError("Files", $"Maksimum {canUpload} Qeder Sekil Yukleye Bilersiniz");
                return View(product);
            }

            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    if (!file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Yalniz JPG Olmalidir");
                        return View(product);
                    }

                    if (!file.CheckFileLength(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Yalniz 300 kbdan az Olmalidir");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CraeteFileAsync(_env, "assets", "img", "product"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);
                }

                dbProduct.ProductImages.AddRange(productImages);
            }

            if (product.MainFile != null)
            {
                if (!product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz JPG Olmalidir");
                    return View(product);
                }

                if (!product.MainFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz 300 kb Olmalidir");
                    return View(product);
                }

                FileHelper.DeleteFile(dbProduct.MainImage, _env, "assets", "img", "product");

                dbProduct.MainImage = await product.MainFile.CraeteFileAsync(_env, "assets", "img", "product");
            }
            AppUser appUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == (User.Identity.Name.ToLowerInvariant()));
            dbProduct.Title = product.Title;
            dbProduct.Description= product.Description;
            dbProduct.Category = product.Category;
            dbProduct.UpdatedBy =$" { appUser.Name} { appUser.SurName}" ;
            dbProduct.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbProduct.Count = product.Count;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteImage(int? id, int? imageId)
        {
            if (id == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            if (product.ProductImages?.Count() <= 1)
            {
                return BadRequest();
            }

            if (!product.ProductImages.Any(p => p.Id == imageId)) return BadRequest();

            product.ProductImages.FirstOrDefault(p => p.Id == imageId).IsDeleted = true;

            await _context.SaveChangesAsync();

            FileHelper.DeleteFile(product.ProductImages.FirstOrDefault(p => p.Id == imageId).Image, _env, "assets", "img", "product");

            List<ProductImage> productImages = product.ProductImages.Where(pi => pi.IsDeleted == false).ToList();

            return PartialView("_ProductImagePartial", productImages);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (product == null) return NotFound();
            AppUser appUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == (User.Identity.Name.ToLowerInvariant()));

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow.AddHours(4);
            product.DeletedBy = $"{appUser.Name}{appUser.SurName}";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
