using Microsoft.AspNetCore.Mvc;

namespace ELearn.Areas.Admin.Controllers
{
    [Area("Manage")]
    public class DashboardController : Controller
    {
        public  IActionResult Index()
        {
            return View();
        }
    }
}
