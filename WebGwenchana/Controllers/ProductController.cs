using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Drawing.Printing;
using System.Linq;
using System.Net.WebSockets;
using WebGwenchana.Data;
using WebGwenchana.Models;

namespace WebGwenchana.Controllers
{
	public class ProductController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ProductController(ApplicationDbContext context)
		{
			_context = context;
		}

		[Route("/shop.html", Name="ShopProduct")]
		public IActionResult Index(int? page,int SortID = 0)
		{
			try
			{
				var pageNumber = page == null || page <= 0 ? 1 : page.Value;
				var pageSize = 12;
				List<Product> LsProducts = new List<Product>();
				if (SortID == 1)
				{
					LsProducts = _context.Products
						.AsNoTracking()
						.OrderBy(x => x.ProductPrice)
						.ToList();
				} else if (SortID == 2)
				{
					LsProducts = _context.Products
						.AsNoTracking()
						.OrderByDescending(x => x.ProductPrice)
						.ToList();
				}
				else if (SortID == 3)
				{
					LsProducts = _context.Products
						.AsNoTracking()
						.OrderByDescending(x => x.ProductDateCreated)
						.ToList();
				}
				else
				{
					LsProducts = _context.Products.AsNoTracking().ToList();
				}
				PagedList<Product> models = new PagedList<Product>(LsProducts.AsQueryable(), pageNumber, pageSize);
				ViewBag.CurrentPage = pageNumber;
				ViewData["DanhMuc"] = _context.Categories.ToList();
                return View(models);
			} catch
			{
                return RedirectToAction("Index", "Home");
            }
        }

		public IActionResult Sort(int SortID = 0)
		{
			var url = $"/shop.html?SortID={SortID}";
			return Json(new { status = "success", redirectUrl = url });
		}

		[Route("danhmuc/{id}", Name = "ListProduct")]
		public IActionResult List (int id, int page=1)
		{
			try
			{
                var pageSize = 10;
				var danhmuc = _context.Categories.Find(id);
                var LsProducts = _context.Products.AsNoTracking().Where(x => x.CatID == id).OrderByDescending(x => x.ProductDateCreated);
                PagedList<Product> models = new PagedList<Product>(LsProducts, page, pageSize);
                ViewBag.CurrentPage = page;
				ViewBag.CurrentCat = danhmuc;
				ViewData["DanhMuc"] = _context.Categories.ToList();
				return View(models);
            } catch
			{
				return RedirectToAction("Index", "Home");
			}
			
		}
		[Route("/{id}.html", Name="ProductDetails")]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Index");
            }
			try
			{

				var product = await _context.Products
					.Include(x => x.Category)
					.FirstOrDefaultAsync(m => m.ProductID == id);
				if (product == null)
				{
					return RedirectToAction("Index");
				}

				var lsProduct =_context.Products.AsNoTracking()
					.Where(x => x.CatID == product.CatID && x.ProductID != id && x.Active == true)
					.OrderByDescending(x => x.ProductDateCreated)
					.Take(4).ToList();
				ViewBag.SanPham = lsProduct;
                string[] photos = product.ProductCollection == null ? null: product.ProductCollection.Split(',');
                ViewData["Photos"] = photos;
				var sizes = new SelectList(_context.SizesPrice.Include(x => x.Size).Where(x => x.ProductID == id).Select(x => new { x.SizeId, Name = x.Size.Name }), "SizeId", "Name");
				ViewData["Sizes"] = sizes;
                return View(product);
			} catch
			{
                return RedirectToAction("Index", "Home");
            }
		}
        public async Task<IActionResult> QuickDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            try
            {

                var product = await _context.Products
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(m => m.ProductID == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }

                return PartialView("QuickDetails",product);
            }
            catch
            {
                return View("Index", "Home");
            }
        }
		public async Task<IActionResult> ChangePrice(int? productId, int? sizeId)
		{
			decimal result = 0;
			var price = await _context.SizesPrice.Where(x => x.SizeId== sizeId && x.ProductID == productId).Select(x => x.ProductPrice).FirstOrDefaultAsync();
			result = (decimal)price;
			var result2 = result.ToString("#,##0 VNĐ");
			var responseData = new { Price = result2 };
			return Json(responseData);
		}
    }
}
