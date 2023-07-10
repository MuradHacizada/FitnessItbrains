using FitnessApp1.DAL;
using FitnessApp1.Helpers;
using FitnessApp1.Models;
using FitnessApp1.Utilities.Extensions;
using FitnessApp1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TrainerController : Controller
    {
        #region Employee
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public TrainerController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index(int page = 0)
        {
            List<Trainer> employees = await _db.Trainers.Skip(page * 5).Take(5).Include(p => p.Position).ToListAsync();
            PaginateVM<Trainer> paginate = new PaginateVM<Trainer>
            {
                Items = employees,
                TotalPage = Math.Ceiling((decimal)_db.Trainers.Count() / 5),
                CurrentPage = page
            };
            return View(paginate);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Position = await _db.Positions.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer)
        {
            ViewBag.Position = await _db.Positions.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            //if (trainer.PositionId == null) { ModelState.AddModelError("PositionId", "Xahis olunur Position secin"); return View(); }
            //bool result = await _db.Positions.AnyAsync(p => p.Id == trainer.PositionId);
            //if (!result) { ModelState.AddModelError("PositionId", "Bele Position Yoxdur"); return View(); }

            if (trainer.Photo == null) { ModelState.AddModelError("Photo", "Zehmet olmasa sekil secin"); return View(); }
            if (!trainer.Photo.CheckFileType("image/")) { ModelState.AddModelError("Photo", "Sekil tipi uygun deyil"); return View(); }
            if (!trainer.Photo.CheckFileSize(400)) { ModelState.AddModelError("Photo", "Sekil uzunlugu uygun deyil"); return View(); }
            trainer.Image = await trainer.Photo.CreateFileAsync(_env.WebRootPath, "img/experienced-trainer");
           
            await _db.Trainers.AddAsync(trainer);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Position = await _db.Positions.ToListAsync();
            if (id < 1 || id == null) { return BadRequest(); }
            Trainer existed = await _db.Trainers.Include(p => p.Position).FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) { return NotFound(); }
            return View(existed);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Trainer trainer)
        {
            ViewBag.Position = await _db.Positions.ToListAsync();
            if (id < 1 || id == null) { return BadRequest(); }
            Trainer existed = await _db.Trainers.Include(p => p.Position).FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) { return NotFound(); }
            bool result = await _db.Positions.AnyAsync(p => p.Id == trainer.PositionId);
            if (!result) { ModelState.AddModelError("Position", "Bele Position Yoxdur"); return View(); }
            if (trainer.Photo != null)
            {
                if (!trainer.Photo.CheckFileType("image/")) { ModelState.AddModelError("Photo", "Sekil tipi uygun deyil"); return View(); }
                if (!trainer.Photo.CheckFileSize(400)) { ModelState.AddModelError("Photo", "Sekil uzunlugu uygun deyil"); return View(); }
                existed.Image.DeleteFile(_env.WebRootPath, "img/experienced-trainer");
                existed.Image = await trainer.Photo.CreateFileAsync(_env.WebRootPath, "img/experienced-trainer");
            }
            if (trainer.PositionId == 0)
            {
                trainer.PositionId = null;

            }

            existed.Name = trainer.Name;
            existed.Surname = trainer.Surname;
            existed.PositionId = trainer.PositionId;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id < 1 || id == null) { return BadRequest(); }
            Trainer existed = await _db.Trainers.FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) { return NotFound(); }
            existed.Image.DeleteFile(_env.WebRootPath, "img/experienced-trainer");
            _db.Trainers.Remove(existed);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
