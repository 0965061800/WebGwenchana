using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebGwenchana.Data;
using WebGwenchana.Models;
using WebGwenchana.ModelViews;

namespace WebGwenchana.Controllers
{
    public class HomeController : Controller
    {
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
		{
			_logger = logger;
			_context = context;
		}

		public IActionResult Index()
		{
			HomeViewVM model = new HomeViewVM();

			var lsProducts = _context.Products.AsNoTracking()
				.Where(x => x.Active == true)
				.OrderByDescending(x => x.ProductDateCreated)
				.Take(8)
				.ToList();

			var bestSeller = _context.Products.AsNoTracking()
				.Where(x => x.Active == true)
				.Where(x => x.BestSellers==true)
				.OrderByDescending(x => x.ProductDateCreated)
				.Take(8)
				.ToList();
			List<ProductHomeVM> lsProductViews = new List<ProductHomeVM>();

		
			var lsCats = _context.Categories
				.AsNoTracking()
				.OrderByDescending(x => x.Order)
				.ToList();

			foreach (var item in lsCats)
			{
				ProductHomeVM productHome = new ProductHomeVM();
				productHome.category = item;
				productHome.lsProducts = lsProducts.Where(x => x.CatID == item.CatID).ToList();
				productHome.bestSeller = bestSeller.Where(x => x.CatID == item.CatID).ToList();
				lsProductViews.Add(productHome);
				model.Products = lsProductViews;
				ViewBag.AllProducts = lsProducts;
				ViewBag.AllBestSeller = bestSeller;  
			}
			return View(model);
		}



		public IActionResult About()
        {
            return View();
        }
		public IActionResult Contact()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
