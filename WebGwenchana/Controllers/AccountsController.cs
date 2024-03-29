﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebGwenchana.Data;
using WebGwenchana.Extension;
using WebGwenchana.Helper;
using WebGwenchana.Models;
using WebGwenchana.ModelViews;


namespace WebGwenchana.Controllers
{
	[Authorize]
	public class AccountsController : Controller
	{
		private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }
		public AccountsController(ApplicationDbContext context, INotyfService notyfService)
		{
			_context = context;
			_notyfService = notyfService;
        }
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ValidatePhone(string Phone)
		{
			try
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
				if (khachhang != null)
					return Json(data: "Số điện thoại : " + Phone + "đã được sử dụng");

				return Json(data: true);

			}
			catch
			{
				return Json(data: true);
			}
		}
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ValidateEmail(string Email)
		{
			try
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
				if (khachhang != null)
					return Json(data: "Email : " + Email + " đã được sử dụng");
				return Json(data: true);
			}
			catch
			{
				return Json(data: true);
			}
		}
		[Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
		public IActionResult Dashboard()
		{
			var taikhoanID = HttpContext.Session.GetString("CustomerID");
			if (taikhoanID != null)
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerID == Convert.ToInt32(taikhoanID));
				if (khachhang != null)
				{
					var lsDonHang = _context.Orders
					   .Include(x => x.TransactStatus)
					   .AsNoTracking()
					   .Where(x => x.CustomerID == khachhang.CustomerID)
					   .OrderByDescending(x => x.OrderDate)
					   .ToList();
					ViewBag.DonHang = lsDonHang;
					return View(khachhang);
				}

			}
			return RedirectToAction("Login");
		}
		[HttpGet]
		[AllowAnonymous]
		[Route("dang-ky.html", Name = "DangKy")]
		public IActionResult DangkyTaiKhoan()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("dang-ky.html", Name = "DangKy")]
		public async Task<IActionResult> DangkyTaiKhoan(RegisterViewModel taikhoan)
		{
			try
			{
				if (ModelState.IsValid)
				{
					string salt = Utilities.GetRandomKey();
					Customer khachhang = new Customer
					{
						FullName = taikhoan.FullName,
						Phone = taikhoan.Phone.Trim().ToLower(),
						Email = taikhoan.Email.Trim().ToLower(),
						Password = Extension.HashMD5.GetMD5Hash(taikhoan.Password + salt.Trim()),
						Active = true,
						Salt = salt,
						CreateDate = DateTime.Now
					};
					try
					{
						_context.Add(khachhang);
						await _context.SaveChangesAsync();
						//Lưu Session MaKh
						HttpContext.Session.SetString("CustomerID", khachhang.CustomerID.ToString());
						var taikhoanID = HttpContext.Session.GetString("CustomerID");

						//Identity
						var claims = new List<Claim>
						{
							new Claim(ClaimTypes.Name,khachhang.FullName),
							new Claim("CustomerID", khachhang.CustomerID.ToString())
						};
						ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
						ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
						await HttpContext.SignInAsync(claimsPrincipal);
						_notyfService.Success("Đăng ký thành công");
						return RedirectToAction("Dashboard", "Accounts");
					}
					catch
					{
						return RedirectToAction("DangkyTaiKhoan", "Accounts");
					}
				}
				else
				{
					return View(taikhoan);
				}
			}
			catch
			{
				return View(taikhoan);
			}
		}
        [HttpGet]
        [AllowAnonymous]
		[Route("dang-nhap.html", Name = "DangNhap")]
		public async Task<IActionResult> Login(string returnUrl)
		{
			var taikhoanID = HttpContext.Session.GetString("CustomerID");
			if (taikhoanID != null)
			{
				return RedirectToAction("Dashboard", "Accounts");
			}
            return View();
		}

        [HttpPost]
		[AllowAnonymous]
		[Route("dang-nhap.html", Name = "DangNhap")]
		public async Task<IActionResult> Login(LoginViewModel customer, string returnUrl = "/shop.html")
		{
			try
			{
				if (ModelState.IsValid)
				{
					bool isEmail = Utilities.IsValidEmail(customer.UserName);
					if (!isEmail) return View(customer);

					var khachhang = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email.Trim() == customer.UserName);

					if (khachhang == null) {
                        _notyfService.Error("Tài khoản không tồn tại");
                        return View(customer);
					}
					
					string pass = Extension.HashMD5.GetMD5Hash(customer.Password + khachhang.Salt.Trim());
					if (khachhang.Password != pass)
					{
						_notyfService.Error("Mật khẩu chưa chính xác");
						return View(customer);
					}
					//kiem tra xem account co bi disable hay khong

					if (khachhang.Active == false)
					{
						return RedirectToAction("ThongBao", "Accounts");
					}

					//Luu Session MaKh
					HttpContext.Session.SetString("CustomerID", khachhang.CustomerID.ToString());
					var taikhoanID = HttpContext.Session.GetString("CustomerID");

					//Identity
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, khachhang.FullName),
						new Claim("CustomerID", khachhang.CustomerID.ToString())
					};
					ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
					ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
					await HttpContext.SignInAsync(claimsPrincipal);
					_notyfService.Success("Đăng nhập thành công");
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                    	return RedirectToAction("Dashboard", "Accounts");
                    }
                    else
                    {
                    	return Redirect(returnUrl);
                    }
                }
			}
			catch
			{
				return RedirectToAction("DangkyTaiKhoan", "Accounts");
			}
			return View(customer);
		}

        [HttpGet]
		[Route("dang-xuat.html", Name = "DangXuat")]
		public IActionResult Logout()
		{
			HttpContext.SignOutAsync();
			HttpContext.Session.Remove("CustomerID");
			return RedirectToAction("Index", "Home");
		}

        [Route("thay-doi-mat-khau.html", Name = "ThayDoiMatKhau")]
        public IActionResult ChangePassword(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerID");
            if (taikhoanID == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            return View();
        }

        [HttpPost]
        [Route("thay-doi-mat-khau.html", Name = "ThayDoiMatKhau")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                 var taikhoanID = HttpContext.Session.GetString("CustomerID");
                if (ModelState.IsValid)
                {
                    var taikhoan =_context.Customers.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return View("Login");
                    var pass = Extension.HashMD5.GetMD5Hash(model.PasswordNow.Trim() + taikhoan.Salt.Trim());

                    {
						string passnew = Extension.HashMD5.GetMD5Hash(model.Password.Trim() + taikhoan.Salt.Trim());
                        taikhoan.Password = passnew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notyfService.Success("Đổi mật khẩu thành công");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Error("Thay đổi mật khẩu không thành công");
                return RedirectToAction("ChangePassword", "Accounts");
            }
            _notyfService.Success("Thay đổi mật khẩu không thành công");
            return RedirectToAction("ChangePassword", "Accounts");
        }
    }
}
