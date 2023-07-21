using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator,Admin")]

    public class PositionController : Controller
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Position> positions = await _context.Positions.ToListAsync();
            return View(positions);
        }
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Position position)
        {
            try
            {
                bool isExist = await _context.Positions.AnyAsync(m => m.Name.Trim() == position.Name.Trim());

                if (isExist)
                {
                    ModelState.AddModelError("Name", "position already exist");
                    return View();
                }


                await _context.Positions.AddAsync(position);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                Position position = await _context.Positions.FirstOrDefaultAsync(m => m.Id == id);

                if (position is null) return NotFound();

                return View(position);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Position position)
        {
            try
            {
                Position dbPos = await _context.Positions.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbPos is null) return NotFound();

                if (dbPos.Name.ToLower().Trim() == position.Name.ToLower().Trim())
                {
                    return RedirectToAction(nameof(Index));
                }

                dbPos.Name = position.Name;

                _context.Positions.Update(position);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }

        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Position db = _context.Positions.Find(id);
            if (db == null) return NotFound();
            _context.Positions.Remove(db);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
