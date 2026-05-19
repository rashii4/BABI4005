using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


[Authorize]
public class MoodController : Controller
{
    private readonly AppDbContext _context;

    public MoodController(AppDbContext context)
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

    // 📋 PAGE
    public IActionResult Index()
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var moods = _context.Moods
            .Where(m => m.StudentID == studentID.Value)
            .OrderByDescending(m => m.EntryDate)
            .ToList();

        ViewBag.Error = TempData["Error"];
        return View(moods);
    }

    // ➕ ADD
    [HttpPost]
    public IActionResult Add(int mood, string notes)
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (!new[] { 1, 2, 3, 4, 5 }.Contains(mood))
        {
            TempData["Error"] = "Invalid mood selection.";
            return RedirectToAction("Index");
        }

        _context.Moods.Add(new Mood
        {
            StudentID = studentID.Value,
            MoodValue = mood,
            Notes = notes,
            EntryDate = DateTime.Now
        });

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

        var entry = _context.Moods
            .FirstOrDefault(x => x.MoodID == id && x.StudentID == studentID.Value);

        if (entry != null)
        {
            _context.Moods.Remove(entry);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    // ✏️ EDIT
    [HttpPost]
    public IActionResult Edit(int moodID, int mood, string notes, string date)
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var entry = _context.Moods
            .FirstOrDefault(x => x.MoodID == moodID && x.StudentID == studentID.Value);

        if (entry != null &&
            DateTime.TryParse(date, out DateTime newDate) &&
            new[] { 1, 2, 3, 4, 5 }.Contains(mood))
        {
            var time = entry.EntryDate.TimeOfDay;
            entry.EntryDate = newDate.Date + time;

            entry.MoodValue = mood;
            entry.Notes = notes;

            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}