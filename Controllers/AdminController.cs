using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    private const string AdminUserId = "b6b6cd21-7a46-4aea-bddd-3ba91e35117f";

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != AdminUserId)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.StudentCount = _context.Students.Count();
        ViewBag.MoodCount = _context.Moods.Count();
        ViewBag.SleepCount = _context.Sleeps.Count();
        ViewBag.DiaryCount = _context.Diaries.Count();

        return View();
    }
}