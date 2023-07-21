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

    public class ArchiveController : Controller
    {
        private readonly AppDbContext _context;

        public ArchiveController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Change> changes = _context.Changes.Include(c=>c.Trainer).ToList();
            List<Order> orders = _context.Order.Include(o=>o.AppUser).Where(o => o.Status == true).ToList();
            ViewBag.Orders = orders;
            return View(changes);
        }
    }
}
