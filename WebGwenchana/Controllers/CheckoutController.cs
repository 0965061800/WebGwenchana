using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebGwenchana.Data;
using WebGwenchana.Extension;
using WebGwenchana.Helper;
using WebGwenchana.Models;
using WebGwenchana.ModelViews;
using WebGwenchana.Services;

namespace WebGwenchana.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notyfService { get; }

		private readonly IVnPayService _vnPayService;

		public CheckoutController(ApplicationDbContext context, INotyfService notyfService, IVnPayService vnpayService)
		{
			_context = context;
			_notyfService = notyfService; 
			_vnPayService = vnpayService;
		}
		public List<CartItem> GioHang
		{
			get
			{
				var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
				if (gh == default(List<CartItem>))
				{
					gh = new List<CartItem>();
				}
				return gh;
			}
		}
		[Route("checkout.html", Name = "Checkout")]
		public IActionResult Index(string returnUrl = null)
		{
			//Lay gio hang ra de xu ly
			var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
			var taikhoanID = HttpContext.Session.GetString("CustomerID");
			MuaHangVM model = new MuaHangVM();
			if (taikhoanID != null)
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerID == Convert.ToInt32(taikhoanID));
				model.CustomerID = khachhang.CustomerID;
				model.FullName = khachhang.FullName;
				model.Email = khachhang.Email;
				model.Phone = khachhang.Phone;
				model.Address = khachhang.Address;
			} else
			{
				return RedirectToAction("Login", "Accounts");
			}
			//ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location", "Name");
			ViewBag.GioHang = cart;
			return View(model);
		}

		[HttpPost]
		[Route("checkout.html", Name = "Checkout")]
		public IActionResult Index(MuaHangVM muaHang, string payment = "COD")
		{
			//Lay ra gio hang de xu ly
			var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
			var taikhoanID = HttpContext.Session.GetString("CustomerID");
			MuaHangVM model = new MuaHangVM();
			if (taikhoanID != null)
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerID == Convert.ToInt32(taikhoanID));
				model.CustomerID = khachhang.CustomerID;
				model.FullName = khachhang.FullName;
				model.Email = khachhang.Email;
				model.Phone = khachhang.Phone;
				model.Address = khachhang.Address;

				//khachhang.LocationID = muaHang.TinhThanh;
				//khachhang.District = muaHang.QuanHuyen;
				//khachhang.Ward = muaHang.PhuongXa;
				khachhang.Phone = muaHang.Phone;
				khachhang.Address = muaHang.Address;
				_context.Update(khachhang);
				_context.SaveChanges();
			}
			try
			{
				if (ModelState.IsValid)
				{

					if (payment == "Thanh toan VNPAY")
					{
						var vnPayModel = new VnPaymentRequestModel
						{
							Amount = cart.Sum(x => x.TotalMoney),
							CreatedDate = DateTime.Now,
							Description = $"{model.FullName} {model.Email}",
							FullName = model.FullName,
							OrderId = new Random().Next(1000, 100000)
						};
						return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
					}
					//Khoi tao don hang
					Order donhang = new Order();
					donhang.CustomerID = model.CustomerID;
					donhang.Address = model.Address;
					//donhang.LocationId = model.TinhThanh;
					//donhang.District = model.QuanHuyen;
					//donhang.Ward = model.PhuongXa;

					donhang.OrderDate = DateTime.Now;
					donhang.TransactStatusID = 1;//Don hang moi
					donhang.IsDeleted = false;
					donhang.IsPaid = false;
					donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
					_context.Add(donhang);
					_context.SaveChanges();
					//tao danh sach don hang

					foreach (var item in cart)
					{
						OrderDetail orderDetail = new OrderDetail();
						orderDetail.OrderID = donhang.OrderId;
						orderDetail.ProductID = item.product.ProductID;
						orderDetail.Amount = item.amount;
						orderDetail.TotalMoney = donhang.TotalMoney;
						orderDetail.Price = (int?)item.product.ProductPrice;
						orderDetail.CreateDate = DateTime.Now;
						_context.Add(orderDetail);
					}
					_context.SaveChanges();
					//clear gio hang
					HttpContext.Session.Remove("GioHang");
					//Xuat thong bao
					_notyfService.Success("Đơn hàng đặt thành công");
					//cap nhat thong tin khach hang
					return RedirectToAction("Success");


				}
			}
			catch
			{
				//ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location", "Name");
				ViewBag.GioHang = cart;
				return View(model);
			}
			//ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location", "Name");
			ViewBag.GioHang = cart;
			return View(model);
		}
		[Route("dat-hang-thanh-cong.html", Name = "Success")]
		public IActionResult Success()
		{
			try
			{
				var taikhoanID = HttpContext.Session.GetString("CustomerID");
				if (string.IsNullOrEmpty(taikhoanID))
				{
					return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
				}
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerID == Convert.ToInt32(taikhoanID));
				var donhang = _context.Orders
					.Where(x => x.CustomerID == Convert.ToInt32(taikhoanID))
					.OrderByDescending(x => x.OrderDate)
					.FirstOrDefault();
				MuaHangSuccessVM successVM = new MuaHangSuccessVM();
				successVM.FullName = khachhang.FullName;
				successVM.DonHangID = donhang.OrderId;
				successVM.Phone = khachhang.Phone;
				successVM.Address = khachhang.Address;
				//successVM.PhuongXa = GetNameLocation(donhang.Ward.Value);
				//successVM.TinhThanh = GetNameLocation(donhang.District.Value);
				return View(successVM);
			}
			catch
			{
				return View();
			}
		}
		public string GetNameLocation(int idlocation)
		{
			try
			{
				var location = _context.Locations.AsNoTracking().SingleOrDefault(x => x.LocationID == idlocation);
				if (location != null)
				{
					return location.NameWithType;
				}
			}
			catch
			{
				return string.Empty;
			}
			return string.Empty;
		}

		[Authorize]
		public IActionResult Fail()
		{
			return View();
		}

		[Authorize]
		public IActionResult PaymentCallBack()
		{
			var response = _vnPayService.PaymentExcute(Request.Query);
			if (response == null || response.VnPayResponseCode!= "00")
			{
				TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
				return RedirectToAction("Fail");
			}
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerID");
            MuaHangVM model = new MuaHangVM();

            var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerID == Convert.ToInt32(taikhoanID));
            model.CustomerID = khachhang.CustomerID;
            model.FullName = khachhang.FullName;
            model.Email = khachhang.Email;
            model.Phone = khachhang.Phone;
            model.Address = khachhang.Address;

            Order donhang = new Order();
            donhang.CustomerID = model.CustomerID;
            donhang.Address = model.Address;
            //donhang.LocationId = model.TinhThanh;
            //donhang.District = model.QuanHuyen;
            //donhang.Ward = model.PhuongXa;

            donhang.OrderDate = DateTime.Now;
            donhang.TransactStatusID = 6;//Don hang moi
            donhang.IsDeleted = false;
            donhang.IsPaid = true;
            donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
            _context.Add(donhang);
            _context.SaveChanges();
            //tao danh sach don hang

            foreach (var item in cart)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = donhang.OrderId;
                orderDetail.ProductID = item.product.ProductID;
                orderDetail.Amount = item.amount;
                orderDetail.TotalMoney = donhang.TotalMoney;
                orderDetail.Price = (int?)item.product.ProductPrice;
                orderDetail.CreateDate = DateTime.Now;
                _context.Add(orderDetail);
            }
            _context.SaveChanges();
            //clear gio hang
            HttpContext.Session.Remove("GioHang");
            //Xuat thong bao
            _notyfService.Success("Đơn hàng đặt thành công");
            //cap nhat thong tin khach hang

            return RedirectToAction("Success");
        }
	}
}
