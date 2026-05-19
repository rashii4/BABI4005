using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[Authorize]
public class DiaryController : Controller
{
    private readonly AppDbContext _context;

    public DiaryController(AppDbContext context)
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

    public IActionResult Index()
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var entries = _context.Diaries
            .Where(d => d.StudentID == studentID.Value)
            .OrderByDescending(d => d.EntryDate)
            .ToList();

        return View(entries);
    }

    // ➕ ADD
    [HttpPost]
    public IActionResult Add(string title, string content)
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var entry = new Diary
        {
            StudentID = studentID.Value,
            Title = title,
            Content = content,
            EntryDate = DateTime.Now
        };

        _context.Diaries.Add(entry);
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

        var entry = _context.Diaries
            .FirstOrDefault(d => d.EntryID == id && d.StudentID == studentID.Value);

        if (entry != null)
        {
            _context.Diaries.Remove(entry);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    // ✏️ EDIT
    [HttpPost]
    public IActionResult Edit(int entryID, string title, string content, string date)
    {
        int? studentID = GetCurrentStudentID();

        if (studentID == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var entry = _context.Diaries
            .FirstOrDefault(d => d.EntryID == entryID && d.StudentID == studentID.Value);

        if (entry != null)
        {
            entry.Title = title;
            entry.Content = content;

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime newDate))
            {
                entry.EntryDate = newDate.Date + entry.EntryDate.TimeOfDay;
            }

            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}