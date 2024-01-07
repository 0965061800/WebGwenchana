using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebGwenchana.Data;
using WebGwenchana.Models;

namespace WebGwenchana.Areas.Admin.Controllers
{
    [Area("Admin")]
	public class AdminCustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }

        public AdminCustomersController(ApplicationDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;

        }

        // GET: Admin/AdminAccounts
        public IActionResult Index(int page = 1, int LocationID = 0)
        {
            var pageNumber = page;
            var pageSize = 20;
            List<Customer> IsCustomers = new List<Customer>();
            if (LocationID != 0)
            {
                IsCustomers = _context.Customers
                .AsNoTracking()
                .Where(x => x.LocationID == LocationID)
                .Include(x => x.Location)
                .OrderByDescending(x => x.CustomerID).ToList();
            }
            else
            {
                IsCustomers = _context.Customers.AsNoTracking()
                .Include(x => x.Location)
                .OrderByDescending(x => x.CustomerID).ToList();
            }

            PagedList<Customer> models = new PagedList<Customer>(IsCustomers.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentProID = LocationID;
            ViewBag.CurrentPage = pageNumber;

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "LocationID", "Name");
            return View(models);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(p => p.Location)
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,FullName,Birthday,Avatar,Address,Email,Phone,Password,LocationID,Active,CreateDate,Salt,Ward,LastLogin,District")] Customer customer)
        {
            if (id != customer.CustomerID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var khachhang = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerID == id);
                    if (khachhang != null)
                    {
                        khachhang.Active = customer.Active;
                    }
                    _context.Update(khachhang);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật trạng thái khách hàng thành công");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }
    }
}
