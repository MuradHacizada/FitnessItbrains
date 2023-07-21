using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]

    [Authorize(Roles = "Moderator,Admin")]

    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {

            List<Blog> blogs = _context.Blogs.ToList();
            return View(blogs);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Blog db = _context.Blogs.Find(id);
            if (db == null) return NotFound();
            _context.Blogs.Remove(db);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog Blog)
        {
            if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                return View();
            }

            if (!Blog.Photo.CheckFileType("/image"))
            {
                ModelState.AddModelError("Photo", "Sekil Formati secin");
            }

            if (Blog.Photo.CheckFileSize(20000))
            {
                ModelState.AddModelError("Photo", "Sekil 20 mb-dan boyuk ola bilmez");
            }


            string filename = await Blog.Photo.CreateFileAsync(_env.WebRootPath, "img/blog-post/");
            Blog.Image = filename;

            _context.Blogs.Add(Blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Blog blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int? id, Blog Blog)
        {
            if (id == null)
            {
                return NotFound();
            }


            Blog db = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (db == null)
            {
                return NotFound();
            }
            if (Blog.Photo != null)
            {

                if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    return View();
                }

                if (!Blog.Photo.CheckFileType("/image"))
                {
                    ModelState.AddModelError("Photo", "Sekil Formati secin");
                }

                if (Blog.Photo.CheckFileSize(20000))
                {
                    ModelState.AddModelError("Photo", "Sekil 20 mb-dan boyuk ola bilmez");
                }
                FileExtension.DeleteFile(_env.WebRootPath, "img/blog-post", db.Image);
                string filename = await Blog.Photo.CreateFileAsync(_env.WebRootPath, "img/blog-post");
                db.Image = filename;
            }
            else
            {
                db.Image = db.Image;
            }

            Blog existName = _context.Blogs.FirstOrDefault(x => x.Title.ToLower() == Blog.Title.ToLower());

            if (existName != null)
            {
                if (db != existName)
                {
                    ModelState.AddModelError("Title", "Name Already Exist");
                    return View();
                }
            }

            db.Title = Blog.Title;
            db.Description = Blog.Description;
            db.Icon = Blog.Icon;



            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Blog blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
    }
}