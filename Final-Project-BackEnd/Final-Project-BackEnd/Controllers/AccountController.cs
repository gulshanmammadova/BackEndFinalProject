using Final_Project_BackEnd.ViewModels.AccountViewModels;
using Final_Project_BackEnd.DataAccessLayer;
using Final_Project_BackEnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Final_Project_BackEnd.ViewModels.BasketViewModels;
using System.Diagnostics.Metrics;

namespace Final_Project_BackEnd.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM) {
            if (!ModelState.IsValid) return View(registerVM);


            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                FatherName = registerVM.FatherName,
                Email = registerVM.Email,
                UserName = registerVM.UserName
            };

            IdentityResult identityResult = await _userManager.CreateAsync(appUser,registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }
            await _userManager.AddToRoleAsync(appUser, "Member");

            return RedirectToAction("index","home");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            AppUser appUser = await _userManager.Users.Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedEmail == loginVM.Email.Trim().ToUpperInvariant());

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View(loginVM);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, loginVM.RemindMe, true);


            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesabiniz Bloklanib");
                return View(loginVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View(loginVM);
            }

            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                if (appUser.Baskets != null && appUser.Baskets.Count() > 0)
                {
                    List<BasketVM> basketVMs = new List<BasketVM>();

                    foreach (Basket basket1 in appUser.Baskets)
                    {
                        BasketVM basketVM = new BasketVM
                        {
                            Id = (int)basket1.ProductId,
                            Count = basket1.Count,
                        };

                        basketVMs.Add(basketVM);
                    }

                    basket = JsonConvert.SerializeObject(basketVMs);

                    HttpContext.Response.Cookies.Append("basket", basket);
                }
            }
            else
            {
                HttpContext.Response.Cookies.Append("basket", "");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders.Where(o => o.IsDeleted == false))
                .ThenInclude(o => o.OrderItems.Where(oi => oi.IsDeleted == false))
                .ThenInclude(oi => oi.Product)
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses,
                Orders = appUser.Orders
            };

            return View(profileVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddAddress(Address address)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses
            };

            if (!ModelState.IsValid)
            {
                return View(nameof(Profile), profileVM);
            }

            if (address.IsMain && appUser.Addresses != null && appUser.Addresses.Count() > 0 && appUser.Addresses.Any(u => u.IsMain))
            {
                appUser.Addresses.FirstOrDefault(a => a.IsMain).IsMain = false;
            }

            address.UserId = appUser.Id;
            address.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            address.CreatedAt = DateTime.UtcNow.AddHours(4);


            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            TempData["Tab"] = "address";

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(profileVM);
            }

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            appUser.Name = profileVM.Name;
            appUser.SurName = profileVM.SurName;
            appUser.FatherName = profileVM.FatherName;

            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {
                appUser.Email = profileVM.Email;
            }

            if (appUser.NormalizedUserName != profileVM.UserName.Trim().ToUpperInvariant())
            {
                appUser.UserName = profileVM.UserName;
            }

            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }

                return View(profileVM);
            }

            await _signInManager.SignInAsync(appUser, true);

            if (!string.IsNullOrWhiteSpace(profileVM.OldPassword))
            {
                if (!await _userManager.CheckPasswordAsync(appUser, profileVM.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Old Password Yanlisdir");
                    return View(profileVM);
                }

                if (profileVM.OldPassword == profileVM.Password)
                {
                    ModelState.AddModelError("Password", "Sifre Kohne Sifreynen Eyni Ola Bilmez");
                    return View(profileVM);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

                identityResult = await _userManager.ResetPasswordAsync(appUser, token, profileVM.Password);

                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }

                    return View(profileVM);
                }
            }

            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAddress(int? id)
        {
            if (id == null) { return NotFound(); }
            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                            .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            Address deletedAddress=appUser.Addresses.FirstOrDefault(a => a.IsDeleted == false && a.Id==id);
            if (deletedAddress == null) { return NotFound(); }
            Address address = new Address
            {
                City = deletedAddress.City,
                State = deletedAddress.State,
                PostalCode = deletedAddress.PostalCode,
                Country = deletedAddress.Country,
                IsMain = deletedAddress.IsMain,
                Id = deletedAddress.Id
            };
            TempData["Tab"] = "address";
            return RedirectToAction(nameof(Profile),address);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditAddress(Address address)
        { 
            if (address == null || address.Id==null) { return NotFound();}
            AppUser user = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                           .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (user==null)
            {
                return NotFound();
            }
            
            Address deletedAddress = user.Addresses.FirstOrDefault(a => a.IsDeleted == false && a.Id == address.Id);
            if (deletedAddress == null) { return NotFound(); }
            bool isMain=address.IsMain;
            if (isMain)
            {
                IEnumerable<Address> differentAddresses = user.Addresses.Where(a => a.Id != address.Id && !a.IsDeleted && a.IsDeleted);
                foreach (Address differentAddress in differentAddresses)
                {
                    differentAddress.IsMain=false;
                }
            }

            deletedAddress.City = address.City;
            deletedAddress.State = address.State;
            deletedAddress.PostalCode = address.PostalCode;
            deletedAddress.Country = address.Country;
            deletedAddress.IsMain = address.IsMain;
            deletedAddress.Id = address.Id;

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            TempData["Tab"] = "address";


            return RedirectToAction(nameof(Profile));


        }

    }
}
