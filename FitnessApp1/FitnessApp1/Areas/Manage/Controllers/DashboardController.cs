using FitnessApp1.DAL;
using FitnessApp1.Models;
using FitnessApp1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ELearn.Areas.Admin.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator,Admin")]

    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            Revenue revenue = _context.Revenue.FirstOrDefault(r => r.Id == 1);
            #region Calculating
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            DateTime oneWeekAgo = DateTime.Now.AddDays(-7);

            List<Fee> feesMonth = _context.Fees
                .Where(f => f.ChangedTime >= oneMonthAgo && f.ChangedTime <= DateTime.Now)
                .ToList();
            List<Fee> feesWeek = _context.Fees
                .Where(f => f.ChangedTime >= oneWeekAgo && f.ChangedTime <= DateTime.Now)
                .ToList();
            decimal month = 0;
            foreach (var item in feesMonth)
            {
                month += item.Amount;
            }
            decimal week = 0;
            foreach (var item in feesWeek)
            {
                week += item.Amount;
            }
            ViewBag.Month = month;
            ViewBag.Week = week;
            List<İncome> incomeMonth = _context.İncomes.Where(f => f.ChangedTime >= oneMonthAgo && f.ChangedTime <= DateTime.Now)
                .ToList();
            List<İncome> incomeWeek = _context.İncomes.Where(f => f.ChangedTime >= oneWeekAgo && f.ChangedTime <= DateTime.Now)
                .ToList();

            decimal monthc = 0;
            foreach (var item in incomeMonth)
            {
                monthc += item.Amount;
            }
            decimal weekc = 0;
            foreach (var item in incomeWeek)
            {
                weekc += item.Amount;
            }
            ViewBag.Monthc = monthc;
            ViewBag.Weekc = weekc;
            #endregion

            List<Order> orders = _context.Order.Include(o=>o.AppUser).ToList();
            DashboardVM dashboard = new DashboardVM()
            {
                Orders=orders,Revenue=revenue,
                Change=_context.Changes.Include(c=>c.Trainer).OrderByDescending(c=>c.ChangedTime).FirstOrDefault()
            };
            return View(dashboard);
        }
    }
}
