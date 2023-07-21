using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator,Admin")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Service> services = await _context.Services.ToListAsync();
            return View(services);

        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            try
            {
                bool isExist = await _context.Services.AnyAsync(m => m.Name.Trim() == service.Name.Trim());
                if (isExist)
                {
                    ModelState.AddModelError("Name", "Service already exist");
                    return View();
                }
                await _context.Services.AddAsync(service);
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

                Service service = await _context.Services.FirstOrDefaultAsync(m => m.Id == id);

                if (service is null) return NotFound();

                return View(service);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            try
            {


                Service dbService = await _context.Services.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbService is null) return NotFound();

                if (dbService.Name.ToLower().Trim() == dbService.Name.ToLower().Trim())
                {
                    return RedirectToAction(nameof(Index));
                }

                dbService.Name = service.Name;

                _context.Services.Update(service);

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
            Service service = await _context.Services.FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return BadRequest();
            }
            if (service.IsDeactive)
            {
                service.IsDeactive = false;
            }
            else
            {
                service.IsDeactive = true;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
