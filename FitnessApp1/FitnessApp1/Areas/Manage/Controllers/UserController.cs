using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mail;
using System.Net;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public UserController(UserManager<AppUser> userManager, AppDbContext context, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            List<AppUser> users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> Send(string? id)
        {
            if ( id == null) { return BadRequest(); }
            AppUser user = await _userManager.Users.FirstOrDefaultAsync(p => p.Id == id);
            if (user == null) { return NotFound(); }

            return View(user);
        }
        public async Task<IActionResult> SendMail(string id, string message)
        {
            AppUser user =_userManager.Users.FirstOrDefault(o => o.Id == id);

            if (user == null) return Json(new { status = 400 });
            
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("6h7mk8y@code.edu.az", "FitnessApp");
            mail.To.Add(new MailAddress(user.Email));

            mail.Subject = "Message";

            string body = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/MailTemplate/Trainer.html"))
            {
                body = reader.ReadToEnd();
            }
            string aboutText = $"Hello Mr <strong>{user.FullName}</strong>";
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

            return Json(new { status = 200 });
        }

        public async Task<IActionResult> Change(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AppUser db = _userManager.Users.FirstOrDefault(x => x.Id == id);
            if (db == null) return NotFound();
            if (db.IsActive==false)
            {
                db.IsActive = true;
            }
            else
            {
                db.IsActive = false; 
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
