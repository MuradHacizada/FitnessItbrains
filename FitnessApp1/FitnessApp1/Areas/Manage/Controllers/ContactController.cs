using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;
        public ContactController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Contact> contacts = await _context.Contacts.ToListAsync();
            return View(contacts);
        }
        public async Task<IActionResult> Detail(int id)
        {
            Contact contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            return View(contact);
        }
        public async Task<IActionResult> Delete(int id)
        {
            Contact contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            _context.Remove(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
