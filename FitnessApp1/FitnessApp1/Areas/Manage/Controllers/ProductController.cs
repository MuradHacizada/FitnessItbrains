using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Areas.Manage.Controllers
{

    [Area("Manage")]
    //[Authorize(Roles = "SuperAdmin")]

    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            List<Product> Products = _context.Products.Include(p=>p.ProductCategories).ThenInclude(pc=>pc.Category).Include(p=>p.Discount).Include(a => a.ProductImages).ToList();
            return View(Products);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product db = _context.Products.Include(a => a.ProductImages).FirstOrDefault(x => x.Id == id);
            if (db == null) return NotFound();
            db.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            ViewBag.Discounts = _context.Discounts.ToList();
            ViewBag.Categories = _context.Categories.Where(t => !t.IsDeleted).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            bool isExist = await _context.Products.AnyAsync(x => x.Name == product.Name);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This Name is already Exist");
            }
            ViewBag.Discounts = _context.Discounts.ToList();
            ViewBag.Categories = _context.Categories.Where(t => !t.IsDeleted).ToList();
            List<ProductImage> Images = new List<ProductImage>();
            foreach (IFormFile item in product.Photo)
            {

                if (item == null)
                {
                    ModelState.AddModelError("Photo", "Image can not be null");
                    return View();
                }
                if (!item.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Pls Select Image");
                    return View();
                }
                if (!item.CheckFileSize(2000))
                {
                    ModelState.AddModelError("Photo", "Max 2mb");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "img/shop");
                ProductImage productImage = new ProductImage
                {
                    ImageUrl = await item.CreateFileAsync(_env.WebRootPath, "img/shop"),
                };
                Images.Add(productImage);
            }
            product.ProductImages = Images;
            product.ProductImages[0].IsMain = true;
             
            product.IsDeleted = false;

            product.ProductCategories = new List<ProductCategory>();
            if (product.CategoryIds != null)
            {
                foreach (var catid in product.CategoryIds)
                {
                    ProductCategory pTag = new ProductCategory
                    {
                        Product = product,
                        CategoryId = catid
                    };
                    _context.ProductCategories.Add(pTag);
                }
            }
            _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _context.Categories.Where(t => !t.IsDeleted).ToList();
            ViewBag.Discounts = _context.Discounts.ToList();

            Product b = _context.Products.Include(x=>x.Discount).Include(x=>x.ProductCategories).ThenInclude(x=>x.Category).Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);
            if (b == null)
            {
                return NotFound();
            }
            return View(b);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Product product)
        {
            bool isExist = await _context.Products.AnyAsync(x => x.Name == product.Name && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This Name is already Exist");
            }
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _context.Categories.Where(t => !t.IsDeleted).ToList();
            ViewBag.Discounts = _context.Discounts.ToList();

            #region MyRegion
            Product dbProduct = await _context.Products.Include(p => p.ProductImages).Include(p => p.ProductCategories)
                    .ThenInclude(t => t.Category).Include(p=>p.Discount)
                    .Where(c => c.IsDeleted == false)
                    .FirstOrDefaultAsync(b => b.Id == product.Id);
            #endregion
            if (dbProduct == null)
            {
                return View();
            }

            string path = "";
            #region MyRegion
            //if (dbProduct.Photo == null)
            //{
            //    List<ProductImage> imgs = new List<ProductImage>();
            //    foreach (IFormFile item in product.Photo)
            //    {

            //        if (item == null)
            //        {
            //            ModelState.AddModelError("Photo", "Image can not be null");
            //            return View();
            //        }
            //        if (!item.CheckFileType("image/"))
            //        {
            //            ModelState.AddModelError("Photo", "Pls Select Image");
            //            return View();
            //        }
            //        if (!item.CheckFileSize(2000))
            //        {
            //            ModelState.AddModelError("Photo", "Max 2mb");
            //            return View();
            //        }
            //        string folder = Path.Combine(_env.WebRootPath, "img/shop");
            //        ProductImage productImag = new ProductImage
            //        {
            //            ImageUrl = await item.CreateFileAsync(_env.WebRootPath, "img/shop"),
            //        };
            //        imgs.Add(productImag);
            //        dbProduct.ProductImages = imgs;
            //        dbProduct.ProductImages[0].IsMain = true;
            //        _context.SaveChanges();
            //    }
            //} 
            #endregion
            if (product.Photo != null)
            {
                List<ProductImage> mages = new List<ProductImage>();
                foreach (IFormFile item in product.Photo)
                {

                    if (item == null)
                    {
                        ModelState.AddModelError("Photo", "Image can not be null");
                        return View();
                    }
                    if (!item.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Pls Select Image");
                        return View();
                    }
                    if (!item.CheckFileSize(2000))
                    {
                        ModelState.AddModelError("Photo", "Max 2mb");
                        return View();
                    }
                    string folder = Path.Combine(_env.WebRootPath, "img/shop");
                    ProductImage productImage = new ProductImage
                    {
                        ImageUrl = await item.CreateFileAsync(_env.WebRootPath, "img/shop"),
                    };
                    mages.Add(productImage);
                }
                dbProduct.ProductImages.AddRange(mages);
            }

            if (product.Photo == null)
            {
                foreach (var item in dbProduct.ProductImages)
                {
                    item.ImageUrl = item.ImageUrl;
                }
                _context.SaveChanges();
            }
            var existCategories = _context.ProductCategories.Where(x => x.ProductId == product.Id).ToList();
            if (product.CategoryIds != null)
            {
                foreach (var categoryId in product.CategoryIds)
                {
                    var existCategory = existCategories.FirstOrDefault(x => x.CategoryId == categoryId);
                    if (existCategory == null)
                    {
                        ProductCategory bookCategory = new ProductCategory
                        {
                            ProductId = product.Id,
                            CategoryId = categoryId,
                        };

                        _context.ProductCategories.Add(bookCategory);
                    }
                    else
                    {
                        existCategories.Remove(existCategory);
                    }
                }

            }
            _context.ProductCategories.RemoveRange(existCategories);
            if (product.DiscountId == 0)
            {
                product.DiscountId = null;

            }

            dbProduct.Name = product.Name;
            dbProduct.Price = product.Price;
            dbProduct.Count = product.Count;
            dbProduct.IsDeleted = false;
            dbProduct.Description = product.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); ;
        }

        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product Product = _context.Products.Where(b => !b.IsDeleted).FirstOrDefault(b => b.Id == id);
            if (Product == null)
            {
                return NotFound();
            }
            return View(Product);
        }
        public IActionResult dd()
        {

            List<ProductImage> p = _context.ProductImages.ToList();
            foreach (var item in p)
            {
                _context.Remove(item);
            }
            _context.SaveChanges();
            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> DeleteImage(int? proImgId)
        {
            ProductImage productImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.Id == proImgId);
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product package = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return BadRequest();
            }
            if (package.IsDeleted)
            {
                package.IsDeleted = false;
            }
            else
            {
                package.IsDeleted = true;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
