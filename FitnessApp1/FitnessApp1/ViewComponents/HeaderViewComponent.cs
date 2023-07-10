using FitnessApp1.DAL;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp1.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //ViewBag.Bio =v  await Task.FromResult(ViewBag.Bio)   v _context.Bio.FirstOrDefault();
            return View();
        }
    }
}
