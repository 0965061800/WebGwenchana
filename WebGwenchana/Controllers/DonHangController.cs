using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGwenchana.Data;
using WebGwenchana.ModelViews;

namespace WebGwenchana.Controllers
{
	public class DonHangController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notyfService { get; }
		public DonHangController(ApplicationDbContext context, INotyfService notyfService)
		{
			_context = context;
			_notyfService = notyfService;
		}
		[HttpPost]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			try
			{
				var taikhoanID = HttpContext.Session.GetString("CustomerID");
				if (string.IsNullOrEmpty(taikhoanID)) return RedirectToAction("Login", "Accounts");
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerID == Convert.ToInt32(taikhoanID));
				if (khachhang == null) return NotFound();
				var donhang = await _context.Orders
					.Include(x => x.TransactStatus)
					.FirstOrDefaultAsync(m => m.OrderId == id && Convert.ToInt32(taikhoanID) == m.CustomerID);
				if (donhang == null) return NotFound();

				var chitietdonhang = _context.OrderDetails
					.Include(x => x.Product)
					.AsNoTracking()
					.Where(x => x.OrderID == id)
					.OrderBy(x => x.OrderDetailID)
					.ToList();
				XemDonHang donHang = new XemDonHang();
				donHang.DonHang = donhang;
				donHang.ChiTietDonHang = chitietdonhang;
				return PartialView("Details", donHang);

			}
			catch
			{
				return NotFound();
			}
		}
	}
}
