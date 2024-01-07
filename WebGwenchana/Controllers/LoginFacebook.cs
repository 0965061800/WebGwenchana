using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebGwenchana.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using WebGwenchana.Data;

namespace WebGwenchana.Controllers
{
    public class LoginFacebook : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }
        public LoginFacebook(ApplicationDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task LoginWithFacebook()
        {
            await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("FacebookResponse")
                });
        }

        public async Task<IActionResult> FacebookResponse()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (result.Succeeded)
                {
                    var claims = result.Principal.Claims;

                    //Retrieve user information
                    var fullName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    //Dang nhap
                    var khachhang = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email.Trim() == email.Trim());

                    if (khachhang != null)
                    {
                        HttpContext.Session.SetString("CustomerID", khachhang.CustomerID.ToString());
                        _notyfService.Success("Đăng nhập thành công");
                    }
                    else
                    {
                        // Dang ky khi chua co tai khoang
                        Customer khachhangnew = new Customer
                        {
                            FullName = fullName,
                            Email = email,
                            Active = true,
                            CreateDate = DateTime.Now
                        };
                        try
                        {
                            _context.Add(khachhangnew);
                            await _context.SaveChangesAsync();
                            //Lưu Session MaKh
                            HttpContext.Session.SetString("CustomerID", khachhangnew.CustomerID.ToString());
                            _notyfService.Success("Đăng ký thành công");
                        }
                        catch
                        {
                            return RedirectToAction("DangkyTaiKhoan", "Accounts");
                        }
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                _notyfService.Error("Đăng nhập không thành công");
                return RedirectToAction("Login", "Accounts");
            }

        }
    }
}
