using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebGwenchana.Data;
using WebGwenchana.Models;
using WebGwenchana.ModelViews;

namespace WebGwenchana.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AccountController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notyfService { get; }
		public AccountController(ApplicationDbContext context, INotyfService notyfService)
		{
			_context = context;
			_notyfService = notyfService;
		}
		// GET: /<controller>/
		public IActionResult Index()
		{
			return View();
		}


		[AllowAnonymous]
		[Route("login.html", Name = "Login")]
		public IActionResult AdminLogin(string returnUrl = null)
		{
			var taikhoanID = HttpContext.Session.GetString("AccountID");
			if (taikhoanID != null) return RedirectToAction("Index", "Home", new { Area = "Admin" });
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}
		[HttpPost]
		[AllowAnonymous]
		[Route("login.html", Name = "Login")]
		public async Task<IActionResult> AdminLogin(LoginViewModel model, string returnUrl = null)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Account kh = _context.Accounts.Include(p => p.Role)
					.SingleOrDefault(p => p.Email.ToLower() == model.UserName.ToLower().Trim());

					if (kh == null)
					{
						ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
					}
					string pass = (model.Password.Trim());
					// + kh.Salt.Trim()
					if (kh.Password.Trim() != pass)
					{
						ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
						return View(model);
					}
					_context.Update(kh);
					await _context.SaveChangesAsync();


					var taikhoanID = HttpContext.Session.GetString("AccountID");
					//identity
					//luuw seccion Makh
					HttpContext.Session.SetString("AccountID", kh.AccountID.ToString());

					//identity
					var userClaims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, kh.FullName),
						new Claim(ClaimTypes.Email, kh.Email),
						new Claim("AccountID", kh.AccountID.ToString()),
						new Claim("RoleId", kh.RoleId.ToString()),
						new Claim(ClaimTypes.Role, kh.Role.RoleName)
					};
					var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
					var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
					await HttpContext.SignInAsync(userPrincipal);



					return RedirectToAction("Index", "Home", new { Area = "Admin" });
				}
			}
			catch
			{
				return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
			}
			return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
		}
		[Route("logout.html", Name = "Logout")]
		public IActionResult AdminLogout()
		{
			try
			{
				HttpContext.SignOutAsync();
				HttpContext.Session.Remove("AccountID");
				return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
			}
			catch
			{
				return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
			}
		}

	}
}
