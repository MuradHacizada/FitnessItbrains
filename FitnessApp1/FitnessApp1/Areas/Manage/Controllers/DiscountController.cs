using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator,Admin")]
    public class DiscountController : Controller
    {
        private readonly AppDbContext _context;

        public DiscountController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Discount> discounts = await _context.Discounts.ToListAsync();
            return View(discounts);

        }
         
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discount discount)
        {
            List<Discount> percent = _context.Discounts.Where(hs => hs.DiscountPercent == discount.DiscountPercent).ToList();
          
            foreach (var item in percent)
            {
                if (item.DiscountPercent == discount.DiscountPercent)
                {
                    ModelState.AddModelError("DiscountPercent", "Percent already have.Write other percent");
                    return View(discount);
                }
            }
            if (!(discount.DiscountPercent >= 0 && discount.DiscountPercent <= 100))
            {
                ModelState.AddModelError("DiscountPercent", "Percent must be between 0 and 100");
                return View(discount);
            }
            _context.Discounts.Add(discount);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                Discount discount = await _context.Discounts.FirstOrDefaultAsync(m => m.Id == id);

                if (discount is null) return NotFound();

                return View(discount);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Discount discount)
        {
            Discount percent = await _context.Discounts.FirstOrDefaultAsync(hs => hs.DiscountPercent == discount.DiscountPercent);
            if (!ModelState.IsValid)
            {
                return View();
            }
            Discount existedCampaign = await _context.Discounts.FirstOrDefaultAsync(c => c.Id == discount.Id);
            if (existedCampaign == null)
            {
                return NotFound();
            }

            if (percent != null && percent.Id != id)
            {
                ModelState.AddModelError("DiscountPercent", "Percent already have.Write other percent");
                return View(existedCampaign);
            }
            if (!(discount.DiscountPercent >= 0 && discount.DiscountPercent <= 100))
            {
                ModelState.AddModelError("DiscountPercent", "Percent must be between 0 and 100");
                return View(discount);
            }
            existedCampaign.DiscountPercent = discount.DiscountPercent;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

    }
}
