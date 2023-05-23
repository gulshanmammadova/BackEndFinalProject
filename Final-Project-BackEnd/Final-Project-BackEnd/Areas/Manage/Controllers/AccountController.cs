using Final_Project_BackEnd.Areas.Manage.ViewModels.AccountVMs;
using Final_Project_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Data;

namespace Final_Project_BackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        //[HttpGet]
        //public async Task<IActionResult> Register()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(RegisterVM registerVM)
        //{
        //    if (!ModelState.IsValid) return View(registerVM);

        //    AppUser appUser = new AppUser
        //    {
        //        Name = registerVM.Name,
        //        SurName = registerVM.SurName,
        //        FatherName = registerVM.FatherName,
        //        Email = registerVM.Email,
        //        UserName = registerVM.UserName
        //    };

        //    IdentityResult identityResult =await _userManager.CreateAsync(appUser, registerVM.Password);

        //    if (!identityResult.Succeeded)
        //    {
        //        foreach (IdentityError identityError in identityResult.Errors)
        //        {
        //            ModelState.AddModelError("", identityError.Description);
        //        }
        //        return View(registerVM);
        //    }

        //    await _userManager.AddToRoleAsync(appUser, "Admin");

        //    //return RedirectToAction(nameof(Login));
        //    return RedirectToAction("index", "dashboard", new { area = "manage" });
        //}

        // [HttpGet]
        // public async Task<IActionResult> CreateRole()
        // {
        //     await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //     await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));

        //     return Content("Ugurlu Oldu");
        //}

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            AppUser appUser = new AppUser
            {
                Name = "Super",
                SurName = "Admin",
                FatherName = "SuperAdminFather",
                UserName = "SuperAdminMain",
                Email = "Superadmin1@gmail.com"
            };

            await _userManager.CreateAsync(appUser, "SuperAdmin133");
            await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

            return Content("Ugurlu Oldu");
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
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email ve ya Sifre Yanlisdir");
                return View(loginVM);
            }


            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, loginVM.RemindMe, true);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email ve ya Sifre Yanlisdir");
                return View(loginVM);
            }

            return RedirectToAction("index", "dashboard", new { areas = "manage" });
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                FatherName = appUser.FatherName,
                Email = appUser.Email,
                UserName = appUser.UserName,
            };

            return View(profileVM);
        }

    }
}
