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

    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Tag> tags = await _context.Tags.ToListAsync();
            return View(tags);

        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag tag)
        {
            try
            {
                


                bool isExist = await _context.Tags.AnyAsync(m => m.Name.Trim() == tag.Name.Trim());

                if (isExist)
                {
                    ModelState.AddModelError("Name", "Tag already exist");
                    return View();
                }


                await _context.Tags.AddAsync(tag);

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

                Tag tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id);

                if (tag is null) return NotFound();

                return View(tag);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tag tag)
        {
            try
            {
                 

                Tag dbTag = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbTag is null) return NotFound();

                if (dbTag.Name.ToLower().Trim() == tag.Name.ToLower().Trim())
                {
                    return RedirectToAction(nameof(Index));
                }

                dbTag.Name = tag.Name;

                _context.Tags.Update(tag);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
            
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Tag tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return BadRequest();
            }
            if(tag.IsDeactive)
            {
                tag.IsDeactive = false;
            }
            else
            {
                tag.IsDeactive = true;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
