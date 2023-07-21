using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp1.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Contact? msg)
        {            
            Contact contact = new Contact
            {
                Id = msg.Id,
                Name = msg.Name,
                Phone = msg.Phone,
                Email = msg.Email,
                Solution = msg.Solution,
                Message = msg.Message
            };

            _context.Contacts.Add(contact);
            _context.SaveChanges();
            return RedirectToAction("index", "shop");
        }
    }
}
