using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[Authorize]
public class SleepController : Controller
{
    private readonly AppDbContext _context;

    public SleepController(AppDbContext context)
    {
        _context = context;
    }

    // Helper method to safely get the logged-in student's ID
    private int? GetCurrentStudentID()
    {
        var email = User.Identity?.Name;

        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        var student = _context.Students.FirstOrDefault(s => s.Email == email);

        if (student == null)
        {
            student = new Student
            {
                Email = email,
                Password = "",
                Firstname = email.Split('@')[0],
                Lastname = "",
                Institution = ""
            };

            _context.Students.Add(student);
            _context.SaveChanges();
        }

        HttpContext.Session.SetString("StudentID", student.StudentID.ToString());

        return student.StudentID;
    }

    // 📋 LOAD PAGE
    public IActionResult Index()
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var sleeps = _context.Sleeps
            .Where(s => s.StudentID == studentID.Value)
            .OrderByDescending(s => s.EntryDate)
            .ToList();

        ViewBag.Error = TempData["Error"];

        return View(sleeps);
    }

    // ➕ ADD
    [HttpPost]
    public IActionResult Add(int hours, int minutes, int quality, string notes)
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (hours < 0 || hours > 24 || minutes < 0 || minutes >= 60)
        {
            TempData["Error"] = "Invalid time input.";
            return RedirectToAction("Index");
        }

        if (!new[] { 1, 2, 3, 4 }.Contains(quality))
        {
            TempData["Error"] = "Invalid sleep quality.";
            return RedirectToAction("Index");
        }

        double totalHours = Math.Round(hours + (minutes / 60.0), 2);

        var sleep = new Sleep
        {
            StudentID = studentID.Value,
            HoursSlept = totalHours,
            SleepQuality = quality,
            Notes = notes,
            EntryDate = DateTime.Now
        };

        _context.Sleeps.Add(sleep);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // 🗑 DELETE
    public IActionResult Delete(int id)
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var entry = _context.Sleeps
            .FirstOrDefault(s => s.SleepID == id && s.StudentID == studentID.Value);

        if (entry != null)
        {
            _context.Sleeps.Remove(entry);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    // ✏️ EDIT
    [HttpPost]
    public IActionResult Edit(int sleepID, int hours, int minutes, int quality, string notes, string date)
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (
            hours >= 0 && hours <= 24 &&
            minutes >= 0 && minutes < 60 &&
            new[] { 1, 2, 3, 4 }.Contains(quality) &&
            !string.IsNullOrEmpty(date) &&
            DateTime.TryParse(date, out DateTime newDate)
        )
        {
            double totalHours = Math.Round(hours + (minutes / 60.0), 2);

            var entry = _context.Sleeps
                .FirstOrDefault(s => s.SleepID == sleepID && s.StudentID == studentID.Value);

            if (entry != null)
            {
                var time = entry.EntryDate.TimeOfDay;
                entry.EntryDate = newDate.Date + time;

                entry.HoursSlept = totalHours;
                entry.SleepQuality = quality;
                entry.Notes = notes;

                _context.SaveChanges();
            }
        }

        return RedirectToAction("Index");
    }
}