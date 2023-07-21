using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace FitnessApp1.Areas.Manage.Controllers
{

    [Area("Manage")]
    [Authorize(Roles = "Moderator,Admin")]


    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<Product> Products = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).Include(p => p.Discount).Include(a => a.ProductImages).ToList();

            return View(Products);
        }
        public ActionResult DownloadProductsCsv()
        {
            List<Product> products = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).Include(p => p.Discount).Include(a => a.ProductImages).ToList();

            var csvContent = new StringBuilder();
            csvContent.AppendLine("Id,Name,Price,Count,Description,IsDeleted");

            foreach (var product in products)
            {
                csvContent.AppendLine($"{product.Id},{product.Name},{product.Price},{product.Count},{product.Description},{product.IsDeleted}");
            }

            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);

            streamWriter.Write(csvContent.ToString());
            streamWriter.Flush();
            memoryStream.Position = 0;

            string csvName = "products.csv";

            return File(memoryStream, "text/csv", csvName);
        }
        public ActionResult DownloadProductsExcel()
        {

            List<Product> products = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).Include(p => p.Discount).Include(a => a.ProductImages).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Ürünler");

                worksheet.Cells.LoadFromCollection(products, true);

                var memoryStream = new MemoryStream();
                package.SaveAs(memoryStream);

                memoryStream.Position = 0;
                string excelName = "products.xlsx";

                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public ActionResult DownloadProductsPdf()
        {
            List<Product> products = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).Include(p => p.Discount).Include(a => a.ProductImages).ToList();


            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12, XFontStyle.Regular);

                var yPos = 0;

                foreach (var product in products)
                {
                    gfx.DrawString($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}, Description: {product.Description}, Isdeleted: {product.IsDeleted}, Count: {product.Count}",

                        font, XBrushes.Black, new XRect(0, yPos, page.Width, page.Height), XStringFormats.TopLeft);
                    yPos += 20;
                }

                var memoryStream = new MemoryStream();
                document.Save(memoryStream);

                memoryStream.Position = 0;
                string pdfName = "products.pdf";

                return File(memoryStream, "application/pdf", pdfName);
            }
        }
        #region MyRegion

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
            if (product == null)
            {
                ModelState.AddModelError("", "Not be empty");
                return View();
            }
            if (product.Name == null)
            {
                ModelState.AddModelError("Name", "");
                return View();
            }
            if (product.Description == null)
            {
                ModelState.AddModelError("Description", "");
                return View();
            }
            if (product.Price == null)
            {
                ModelState.AddModelError("Price", "");
                return View();
            }
            if (product.Count == null)
            {
                ModelState.AddModelError("Count", "");
                return View();
            }
            ViewBag.Discounts = _context.Discounts.ToList();
            ViewBag.Categories = _context.Categories.Where(t => !t.IsDeleted).ToList();
            if (product.Photo == null)
            {
                ModelState.AddModelError("", "Choose a photo!");
                return View();
            }
            else
            {

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
            }




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
            List<AppUser> subscribes = _userManager.Users.ToList();
            foreach (var sub in subscribes)
            {
                string link = "https://localhost:7208/shop/detail/" + product.Id;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("6h7mk8y@code.edu.az", "Fitness App");
                mail.To.Add(new MailAddress(sub.Email));


                mail.Subject = "Fitness App";
                string body = string.Empty;

                using (StreamReader reader = new StreamReader("wwwroot/MailTemplate/Product.html"))
                {
                    body = reader.ReadToEnd();
                }

                string about = $"<strong>Hello</strong><br /> a new <strong>{product.Name} ${product.Price}</strong> product added to our shop <br/>click the link!";
                body = body.Replace("{{link}}", link);
                mail.Body = body.Replace("{{about}}", about);
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;

                smtp.Credentials = new NetworkCredential("6h7mk8y@code.edu.az", "fsxwvuxfycmlctfo");
                smtp.Send(mail);
            }
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

            Product b = _context.Products.Include(x => x.Discount).Include(x => x.ProductCategories).ThenInclude(x => x.Category).Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);
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
                    .ThenInclude(t => t.Category).Include(p => p.Discount)
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
        #endregion
    }
}
