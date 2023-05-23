using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Final_Project_BackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    public class DashboardController : Controller
    {
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
