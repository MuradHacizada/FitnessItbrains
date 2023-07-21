using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator,Admin")]

    public class PackageController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public PackageController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Package> packages = await _context.Packages.ToListAsync();
            return View(packages);
        }
        public IActionResult Create()
        {
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Trainers = _context.Trainers.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Package package)
        {

            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Trainers = _context.Trainers.ToList();
            bool isExist = await _context.Packages.AnyAsync(x => x.Title == package.Title);
            if (isExist)
            {
                ModelState.AddModelError("Title", "This Title is already Exist");
            }
            //if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            //{
            //    return View();
            //}

            if (!package.Photo.CheckFileType("/image"))
            {
                ModelState.AddModelError("Photo", "Sekil Formati secin");
            }

            if (package.Photo.CheckFileSize(20000))
            {
                ModelState.AddModelError("Photo", "Sekil 20 mb-dan boyuk ola bilmez");
            }
            if (package.Photo == null)
            {
                ModelState.AddModelError("Photo", "Sekil Formati secin");
                return View();
            }
            else
            {


                string filename = await package.Photo.CreateFileAsync(_env.WebRootPath, "img/health-care-package");
                package.Image = filename;
            }
            package.PackageTags = new List<PackageTag>();
            if (package.TagIds != null)
            {
                foreach (var tagId in package.TagIds)
                {
                    PackageTag pTag = new PackageTag
                    {
                        Package = package,
                        TagId = tagId
                    };
                    _context.PackageTags.Add(pTag);
                }
            }
            _context.Packages.AddAsync(package);
            package.PackageTrainers = new List<PackageTrainer>();
            if (package.TrainerIds != null)
            {
                foreach (var trainerId in package.TrainerIds)
                {
                    PackageTrainer pTrainer = new PackageTrainer
                    {
                        Package = package,
                        TrainerId = trainerId
                    };
                    _context.PackageTrainers.Add(pTrainer);
                }
            }
            _context.Packages.AddAsync(package);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }


        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Trainers = _context.Trainers.ToList();

            Package b = _context.Packages.Include(x => x.PackageTrainers).ThenInclude(x => x.Trainer).Include(x => x.PackageTags).ThenInclude(p => p.Tag).FirstOrDefault(x => x.Id == id);
            if (b == null)
            {
                return NotFound();
            }
            return View(b);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Package package)
        {
            bool isExist = await _context.Packages.AnyAsync(x => x.Title == package.Title && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "This Name is already Exist");
            }
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Trainers = _context.Trainers.ToList();

            #region MyRegion
            Package dbPackage = await _context.Packages.Include(x => x.PackageTrainers).ThenInclude(x => x.Trainer).Include(x => x.PackageTags).ThenInclude(p => p.Tag).FirstOrDefaultAsync(b => b.Id == package.Id);


            var existTags = _context.PackageTags.Where(x => x.PackageId == package.Id).ToList();
            if (package.TagIds != null)
            {
                foreach (var tagId in package.TagIds)
                {
                    var existTag = existTags.FirstOrDefault(x => x.TagId == tagId);
                    if (existTag == null)
                    {
                        PackageTag bookCategory = new PackageTag
                        {
                            PackageId = package.Id,
                            TagId = tagId,
                        };

                        _context.PackageTags.Add(bookCategory);
                    }
                    else
                    {
                        existTags.Remove(existTag);
                    }
                }

            }
            _context.PackageTags.RemoveRange(existTags);


            var existTrainers = _context.PackageTrainers.Where(x => x.PackageId == package.Id).ToList();
            if (package.TrainerIds != null)
            {
                foreach (var trId in package.TrainerIds)
                {
                    var existTr = existTrainers.FirstOrDefault(x => x.TrainerId == trId);
                    if (existTr == null)
                    {
                        PackageTrainer packageTrainer = new PackageTrainer
                        {
                            PackageId = package.Id,
                            TrainerId = trId,
                        };

                        _context.PackageTrainers.Add(packageTrainer);
                    }
                    else
                    {
                        existTrainers.Remove(existTr);
                    }
                }

            }
            _context.PackageTrainers.RemoveRange(existTrainers);

            #endregion
            if (package.Photo != null)
            {

                if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    return View();
                }

                if (!package.Photo.CheckFileType("/image"))
                {
                    ModelState.AddModelError("Photo", "Sekil Formati secin");
                }

                if (package.Photo.CheckFileSize(20000))
                {
                    ModelState.AddModelError("Photo", "Sekil 20 mb-dan boyuk ola bilmez");
                }
                FileExtension.DeleteFile(_env.WebRootPath, "img/health-care-package", dbPackage.Image);
                string filename = await package.Photo.CreateFileAsync(_env.WebRootPath, "img/health-care-package");
                dbPackage.Image = filename;
            }
            else
            {
                dbPackage.Image = dbPackage.Image;
            }
            dbPackage.Title = package.Title;
            dbPackage.Price = package.Price;
            dbPackage.PriceLife = package.PriceLife;
            dbPackage.PriceYear = package.PriceYear;
            dbPackage.Icon = package.Icon;
            dbPackage.Session = package.Session;
            dbPackage.Training = package.Training;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); ;
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Package db = _context.Packages.Find(id);
            if (db == null) return NotFound();
            _context.Packages.Remove(db);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
