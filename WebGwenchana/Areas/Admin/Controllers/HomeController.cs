using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGwenchana.Data;
using WebGwenchana.Models;

namespace WebGwenchana.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Area("Admin")]
		public IActionResult Index()
        {
            //var taikhoanID = HttpContext.Session.GetString("AccountID");
            //if (taikhoanID == null) return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            return View();
        }
    }
}
