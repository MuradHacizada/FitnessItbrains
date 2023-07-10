using FitnessApp1.DAL;
using FitnessApp1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppDbContext _context;
        public AboutController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            AboutVM aboutVM = new AboutVM()
            {
                Services = await _context.Services.ToListAsync()
            };
            return View(aboutVM);
        }
        public async Task<IActionResult> Trainer()
        {
            AboutVM aboutVM = new AboutVM()
            {
                Trainers = await _context.Trainers.Include(p => p.Position).ToListAsync()
            };
            return View(aboutVM);
        }
    }
}
