using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
   [Authorize(Roles = "Moderator,Admin")]

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
        public async Task<IActionResult> Send(int id, string message)
        {
            Contact contact = _context.Contacts.FirstOrDefault(o => o.Id == id);       


            #region MyRegion
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("6h7mk8y@code.edu.az", "FitnessApp");
            mail.To.Add(new MailAddress(contact.Email));

            mail.Subject = "Order";

            string body = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/MailTemplate/Trainer.html"))
            {
                body = reader.ReadToEnd();
            }
            string aboutText = $"<strong>{contact.Name}</strong>";
            string messageTxt = $"{message}";
            body = body.Replace("{{message}}", messageTxt);
            mail.Body = body.Replace("{{aboutText}}", aboutText);
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("6h7mk8y@code.edu.az", "fsxwvuxfycmlctfo");
            smtp.Send(mail);
            #endregion
 
            return Json(new { status = 200 });


        }
    }
}
