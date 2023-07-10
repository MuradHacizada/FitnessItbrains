using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    //[Authorize(Roles = "Admin,SuperAdmin")]

    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {

            List<Order> orders = _context.Order.ToList();
            return View(orders);
        }

        public IActionResult Edit(int id)
        {
            Order order = _context.Order.Include(o => o.OrderItems).Include(o => o.AppUser).FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        public async Task<IActionResult> Accept(int id, string message)
        {
            Order order = _context.Order.FirstOrDefault(o => o.Id == id);
            AppUser user = await _userManager.FindByIdAsync(order.AppUserId);
            if (order == null) return Json(new { status = 400 });
            order.Status = true;
            order.Message = message;
            _context.SaveChanges();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("6h7mk8y@code.edu.az", "FitnessApp");
            mail.To.Add(new MailAddress(user.Email));

            mail.Subject = "Order";

            string body = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/MailTemplate/OrderMessage.html"))
            {
                body = reader.ReadToEnd();
            }
            string aboutText = $"Hello Mr/Mrs <strong>{user.FullName}</strong>";
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
        public async Task<IActionResult> Reject(int id, string message)
        {
            Order order = _context.Order.FirstOrDefault(o => o.Id == id);
            AppUser user = await _userManager.FindByIdAsync(order.AppUserId);
            if (order == null) return Json(new { status = 400 });
            order.Status = false;
            order.Message = message;
            _context.SaveChanges();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("6h7mk8y@code.edu.az", "FitnessApp");
            mail.To.Add(new MailAddress(user.Email));

            mail.Subject = "Order";

            string body = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/MailTemplate/OrderMessage.html"))
            {
                body = reader.ReadToEnd();
            }
            string aboutText = $"Hello Mr/Mrs <strong>{user.FullName }</strong>";
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

    }
}
