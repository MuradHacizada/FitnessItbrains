using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FitnessApp1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context=context;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Sliders = await _context.Sliders.ToListAsync(),
                Benefits = await _context.Benefits.ToListAsync(),
                Trainers = await _context.Trainers.Include(t => t.Position).Take(4).ToListAsync(),
                Blogs = await _context.Blogs.Take(3).ToListAsync(),
                Packages = await _context.Packages.Include(p => p.PackageTags).ThenInclude(t => t.Tag).Include(p => p.PackageTrainers).ThenInclude(t => t.Trainer).ToListAsync(),
            };
            return View(homeVM);
        }
    }
}