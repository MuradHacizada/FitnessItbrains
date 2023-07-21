using FitnessApp1.DAL;
using FitnessApp1.Helpers;
using FitnessApp1.Models;
using FitnessApp1.Utilities.Extensions;
using FitnessApp1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator,Admin")]

    public class TrainerController : Controller
    {
        #region Employee
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public TrainerController(AppDbContext db, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            _db = db;
            _env = env;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int page = 0)
        {
            List<Trainer> employees = await _db.Trainers.Skip(page * 3).Take(3).Include(p => p.Position).ToListAsync();
            PaginateVM<Trainer> paginate = new PaginateVM<Trainer>
            {
                Items = employees,
                TotalPage = Math.Ceiling((decimal)_db.Trainers.Count() / 3),
                CurrentPage = page
            };
            return View(paginate);
        }

        public async Task<IActionResult> Payment(int? id)
        {
            if (id < 1 || id == null) { return BadRequest(); }
            Trainer existed = await _db.Trainers.FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) { return NotFound(); }

            return View(existed);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(int id, string message, decimal salary)
        {
            string role = "";
            if (User.IsInRole("Admin"))
            {
                role = "Admin";
            } 
            Trainer trainer = _db.Trainers.FirstOrDefault(o => o.Id == id);
            if (trainer == null) return Json(new { status = 400 });
            trainer.Salary += salary;
            Revenue revenue = _db.Revenue.FirstOrDefault(o => o.Id == 1);
            Change change = new Change()
            {
                Message = message,
                Amount = salary,
                TrainerId = trainer.Id,
                ChangedTime = DateTime.UtcNow,
                Role=role
            };
            revenue.Totals -= salary;

            Fee fee = new Fee()
            {
                Amount=salary,
                ChangedTime=DateTime.UtcNow
            };
            _db.Add(change);
            _db.Add(fee);
            _db.SaveChanges();
            #region Mailing
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("6h7mk8y@code.edu.az", "FitnessApp");
            mail.To.Add(new MailAddress(trainer.Email));

            mail.Subject = "Order";

            string body = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/MailTemplate/Trainer.html"))
            {
                body = reader.ReadToEnd();
            }
            string aboutText = $"<strong>{trainer.Name}</strong>";
            string messageTxt = $"{message} and your salary: ${salary}";
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

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> SendMail(int id, string message)
        {
            Trainer trainer = _db.Trainers.FirstOrDefault(o => o.Id == id);

            if (trainer == null) return Json(new { status = 400 });
            _db.SaveChanges();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("6h7mk8y@code.edu.az", "FitnessApp");
            mail.To.Add(new MailAddress(trainer.Email));

            mail.Subject = "Order";

            string body = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/MailTemplate/Trainer.html"))
            {
                body = reader.ReadToEnd();
            }
            string aboutText = $"Hello Mr <strong>{trainer.Name}</strong>";
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
        public async Task<IActionResult> SendMailList(string message)
        {
            List<Trainer> trainers = _db.Trainers.ToList();

            if (trainers == null) return Json(new { status = 400 });

            foreach (var trainer in trainers)
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("6h7mk8y@code.edu.az", "FitnessApp");
                mail.To.Add(new MailAddress(trainer.Email));

                mail.Subject = "Order";

                string body = string.Empty;

                using (StreamReader reader = new StreamReader("wwwroot/MailTemplate/Trainer.html"))
                {
                    body = reader.ReadToEnd();
                }
                string aboutText = $"Hello Mr <strong>{trainer.Name}</strong>";
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
            }

            return Json(new { status = 200 });
        }
        public async Task<IActionResult> Send(int? id)
        {
            if (id < 1 || id == null) { return BadRequest(); }
            Trainer existed = await _db.Trainers.FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) { return NotFound(); }

            return View(existed);
        }
        public async Task<IActionResult> SendList()
        {
            return View();
        }

        #region MyRegion

        public async Task<IActionResult> Create()
        {
            ViewBag.Position = await _db.Positions.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer)
        {
            ViewBag.Position = await _db.Positions.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            //if (trainer.PositionId == null) { ModelState.AddModelError("PositionId", "Xahis olunur Position secin"); return View(); }
            //bool result = await _db.Positions.AnyAsync(p => p.Id == trainer.PositionId);
            //if (!result) { ModelState.AddModelError("PositionId", "Bele Position Yoxdur"); return View(); }

            if (trainer.Photo == null) { ModelState.AddModelError("Photo", "Zehmet olmasa sekil secin"); return View(); }
            if (!trainer.Photo.CheckFileType("image/")) { ModelState.AddModelError("Photo", "Sekil tipi uygun deyil"); return View(); }
            if (!trainer.Photo.CheckFileSize(400)) { ModelState.AddModelError("Photo", "Sekil uzunlugu uygun deyil"); return View(); }
            trainer.Image = await trainer.Photo.CreateFileAsync(_env.WebRootPath, "img/experienced-trainer");

            await _db.Trainers.AddAsync(trainer);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Position = await _db.Positions.ToListAsync();
            if (id < 1 || id == null) { return BadRequest(); }
            Trainer existed = await _db.Trainers.Include(p => p.Position).FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) { return NotFound(); }
            return View(existed);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Trainer trainer)
        {
            ViewBag.Position = await _db.Positions.ToListAsync();
            if (id < 1 || id == null) { return BadRequest(); }
            Trainer existed = await _db.Trainers.Include(p => p.Position).FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) { return NotFound(); }
            bool result = await _db.Positions.AnyAsync(p => p.Id == trainer.PositionId);
            if (!result) { ModelState.AddModelError("Position", "Bele Position Yoxdur"); return View(); }
            if (trainer.Photo != null)
            {
                if (!trainer.Photo.CheckFileType("image/")) { ModelState.AddModelError("Photo", "Sekil tipi uygun deyil"); return View(); }
                if (!trainer.Photo.CheckFileSize(400)) { ModelState.AddModelError("Photo", "Sekil uzunlugu uygun deyil"); return View(); }
                existed.Image.DeleteFile(_env.WebRootPath, "img/experienced-trainer");
                existed.Image = await trainer.Photo.CreateFileAsync(_env.WebRootPath, "img/experienced-trainer");
            }
            if (trainer.PositionId == 0)
            {
                trainer.PositionId = null;

            }

            existed.Name = trainer.Name;
            existed.Surname = trainer.Surname;
            existed.PositionId = trainer.PositionId;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id < 1 || id == null) { return BadRequest(); }
            Trainer existed = await _db.Trainers.FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) { return NotFound(); }
            existed.Image.DeleteFile(_env.WebRootPath, "img/experienced-trainer");
            _db.Trainers.Remove(existed);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion 
        #endregion
    }
}
