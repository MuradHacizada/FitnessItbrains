using FitnessApp1.DAL;
using FitnessApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FitnessApp1.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Moderator,Admin")]

    public class CommentController : Controller
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Comment> comments = _context.Comments.ToList();
            return View(comments);
        }
        public IActionResult Change(int?id)
        {
            Comment comment = _context.Comments.FirstOrDefault(c=>c.Id==id);
            if (comment.IsAccess == true) { comment.IsAccess = false; }
            else
            {
            comment.IsAccess = true;

            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
