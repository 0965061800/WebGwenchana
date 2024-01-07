using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebGwenchana.Data;
using WebGwenchana.Helper;
using WebGwenchana.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using System.Collections;
using Microsoft.CodeAnalysis.Options;
using WebGwenchana.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.Build.Evaluation;
namespace WebGwenchana.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }
        public AdminProductsController(ApplicationDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        // GET: Admin/AdminProducts

        public IActionResult Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 8;
            List<Product> IsProducts = new List<Product>();
            if (CatID != 0)
            {
                IsProducts = _context.Products
                .AsNoTracking()
                .Where(x => x.CatID == CatID)
                .Include(x => x.Category)
                .OrderByDescending(x => x.ProductID).ToList();
            } else
            {
                IsProducts = _context.Products.AsNoTracking()
                .Include(x => x.Category)
                .OrderByDescending(x => x.ProductID).ToList();
            }

            PagedList<Product> models = new PagedList<Product>(IsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;

            ViewBag.CurrentPage = pageNumber;

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatID", "CatName");

            //List<SelectListItem> IsTrangThai = new List<SelectListItem>();
            //IsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "1" });
            //IsTrangThai.Add(new SelectListItem() { Text = "Unactive", Value = "0" });
            //ViewData["IsTrangThai"] = IsTrangThai;

            return View(models);
        }
        public IActionResult Filtter(int CatID = 0)
        {
            var url = $"/Admin/AdminProducts?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/AdminProducts";
            }
            return Json(new { status = "success", redirectUrl = url });
        }
        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            List<SizePrice> sizePrices = _context.SizesPrice.Where(x => x.ProductID == id).Include(x => x.Size).ToList();
            ViewData["SizePriceList"] = sizePrices;
            string[] photos = product.ProductCollection == null ? null : product.ProductCollection.Split(',');
            ViewData["Photos"] = photos;
            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatID", "CatName");
            ViewData["Sizes"] = new SelectList(_context.Sizes, "SizeId", "Name");
            return View("Create");
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,ProductShortDesciption,ProductDescription,CatID,ProductPrice,ProductDiscount,ProductPhoto,ProductTitle,ProductDateCreated,BestSellers,Active,Tags,MetaDesc,MetaKey,UnitsInStock,SizeId")] Product product, IFormFile fThumb, IEnumerable<IFormFile> fCollection, IEnumerable<int> SizeId)
        {
            if (ModelState.IsValid)
            {
                //Đoạn này là xử lý hình ảnh Thumnail để add vào thư mục root
                product.ProductName = Utilities.ToTitleCase(product.ProductName);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.ProductName) + extension;
                    product.ProductPhoto = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(product.ProductPhoto)) product.ProductPhoto = "default.jpg";

                //Đoạn này là xử lý collection
                List<string> productCollection = new List<string> { };
                if (fCollection != null)
                {
                    foreach (IFormFile photo in fCollection)
                    {
                        await Utilities.UploadFile(photo, @"gallery");
                        productCollection.Add(photo.FileName);
                    }
                }
                product.ProductCollection = string.Join(",", productCollection);
                //Đoạn này là xử lý các phần tử mặc định khác
                product.ProductDateCreated = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                //Đoạn này là xử lý Variable Product
                if (SizeId.Count() > 0)
                {
                    foreach (int sizeId in SizeId)
                    {
                        SizePrice newSizePrice = new SizePrice();
                        newSizePrice.SizeId = sizeId;
                        newSizePrice.ProductID = product.ProductID;
                        newSizePrice.ProductPrice = product.ProductPrice;
                        _context.Add(newSizePrice);
                    }
                }
                {
                }
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm sản phẩm thành công");
                return RedirectToAction(nameof(Index));
            } else
            {
                _notyfService.Error("Thêm sản phẩm thất bại");
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatID", "CatName", product.CatID);

            return View(product);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatID", "CatName", product.CatID);
            string[] photos = product.ProductCollection == null ? null : product.ProductCollection.Split(',');
            ViewData["Photos"] = photos;
            ViewData["Sizes"] = new SelectList(_context.Sizes, "SizeId", "Name");
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,ProductShortDesciption,ProductDescription,CatID,ProductPrice,ProductDiscount,ProductPhoto,ProductTitle,ProductDateCreated,BestSellers,Active,Tags,MetaDesc,MetaKey,UnitsInStock,ProductCollection,SizeId")] Product product, IFormFile? fThumb, IEnumerable<IFormFile>? fCollection, int SizeId)
        {

            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.ProductName = Utilities.ToTitleCase(product.ProductName);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(product.ProductName) + extension;
                        product.ProductPhoto = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(product.ProductPhoto)) product.ProductPhoto = "default.jpg";
                    List<string> productCollection = new List<string> { };
                    if (fCollection.Count() > 0 && fCollection != null)
                    {
                        foreach (IFormFile photo in fCollection)
                        {
                            await Utilities.UploadFile(photo, @"gallery");
                            productCollection.Add(photo.FileName);
                        }
                        product.ProductCollection = string.Join(",", productCollection);
                    }
                    if (SizeId != null && SizeId >0)
                    {
                        int oldsize = (int)_context.Products.Where(x => x.ProductID == product.ProductID).Select(x => x.SizeId).FirstOrDefault();
                        SizePrice sp = _context.SizesPrice.Where(x=>x.ProductID == product.ProductID && x.SizeId==oldsize).FirstOrDefault();
                        sp.SizeId = SizeId;
                        _context.Update(sp);
                    }
                    _context.Update(product);
                    _notyfService.Success("Cập nhật thành công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            } else
            {
                _notyfService.Success("Cập nhật thất bại");
            }

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatID", "CatName", product.CatID);
            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa sản phẩm thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

        public IActionResult AddSizePrice(int? id)
        {
            List<SizePrice> sizes = new List<SizePrice>();
            sizes = _context.SizesPrice.Where(x => x.ProductID == id).ToList();
            ViewData["Sizes"] = new SelectList(_context.Sizes, "SizeId", "Name");
            ViewData["productId"] = id;
            return View(sizes);
        }
        [HttpGet]
        public async Task<IActionResult> AddSize(int id, int productID)
        {
            ViewData["Sizes"] = new SelectList(_context.Sizes, "SizeId", "Name");
            ViewData["Id"] = id;
            ViewData["productId"] = productID;
            return PartialView("SizeRow");
        }
        [HttpPost]
        public async Task<IActionResult> SaveSizePrice(List<SizePrice> sizePrices)
        {
            if (sizePrices.Count() > 0)
            {
                foreach(var sizePrice in sizePrices)
                {
                    // Edit đối với sizePrice đã tồn tài
                    if (sizePrice.SizePriceID != 0)
                    {
                        SizePrice sizeprice = _context.SizesPrice.FirstOrDefault(x => x.SizePriceID == sizePrice.SizePriceID);
                        sizeprice.SizeId = sizePrice.SizeId;
                        sizeprice.ProductPrice = sizePrice.ProductPrice;

                    } else
                    // Tạo mới đối với sizePrice chưa tồn tại
                    {
                        _context.SizesPrice.Add(sizePrice);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SizeDelete(int id, int productID)
        {
            var sizePrice = await _context.SizesPrice.FindAsync(id);
            if (sizePrice != null)
            {
                _context.SizesPrice.Remove(sizePrice);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa size thành công");
            return RedirectToAction("AddSizePrice", new {id = productID});
        }
    }
}
