using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

[Authorize]
public class ReportController : Controller
{
    private readonly AppDbContext _context;

    public ReportController(AppDbContext context)
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
        int? userId = GetCurrentStudentID();

        if (userId == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var today = DateTime.Today;

        var dates = Enumerable.Range(0, 7)
            .Select(i => today.AddDays(-6 + i).ToString("yyyy-MM-dd"))
            .ToList();

        var rangeStart = today.AddDays(-6);
        var rangeEnd = today.AddDays(1);

        var allSleeps = _context.Sleeps
            .Where(s => s.StudentID == userId.Value
                     && s.EntryDate >= rangeStart
                     && s.EntryDate < rangeEnd)
            .Select(s => new { s.EntryDate, s.HoursSlept, s.SleepQuality })
            .ToList();

        var allMoods = _context.Moods
            .Where(m => m.StudentID == userId.Value
                     && m.EntryDate >= rangeStart
                     && m.EntryDate < rangeEnd)
            .Select(m => new { m.EntryDate, m.MoodValue })
            .ToList();

        Debug.WriteLine($"Report Index: user={userId.Value} range={rangeStart:yyyy-MM-dd}..{rangeEnd:yyyy-MM-dd} moods={allMoods.Count} sleeps={allSleeps.Count}");

        var dailyMood = new List<double>();
        var dailySleep = new List<double>();
        var dailyQuality = new List<int>();

        foreach (var d in dates)
        {
            var date = DateTime.Parse(d).Date;

            var dayMoods = allMoods.Where(m => m.EntryDate.Date == date).ToList();
            var daySleeps = allSleeps.Where(s => s.EntryDate.Date == date).ToList();

            double mood = dayMoods.Any()
                ? Math.Round(dayMoods.Average(m => (double)m.MoodValue), 2)
                : 0;

            double totalSleep = daySleeps.Any()
                ? Math.Round(daySleeps.Sum(s => s.HoursSlept), 2)
                : 0;

            int avgQuality = daySleeps.Any()
                ? (int)Math.Round(daySleeps.Average(s => (double)s.SleepQuality))
                : 0;

            dailyMood.Add(mood);
            dailySleep.Add(totalSleep);
            dailyQuality.Add(avgQuality);
        }

        ViewBag.Dates = dates;
        ViewBag.Mood = dailyMood;
        ViewBag.Sleep = dailySleep;
        ViewBag.Quality = dailyQuality;

        var earliestMood = _context.Moods
            .Where(m => m.StudentID == userId.Value)
            .Min(m => (DateTime?)m.EntryDate);

        var earliestSleep = _context.Sleeps
            .Where(s => s.StudentID == userId.Value)
            .Min(s => (DateTime?)s.EntryDate);

        DateTime earliest = today;

        if (earliestMood.HasValue && earliestSleep.HasValue)
        {
            earliest = earliestMood.Value.Date < earliestSleep.Value.Date
                ? earliestMood.Value.Date
                : earliestSleep.Value.Date;
        }
        else if (earliestMood.HasValue)
        {
            earliest = earliestMood.Value.Date;
        }
        else if (earliestSleep.HasValue)
        {
            earliest = earliestSleep.Value.Date;
        }

        int totalWeeks = (int)Math.Ceiling((today - earliest).TotalDays / 7.0);
        totalWeeks = Math.Max(totalWeeks, 1);

        ViewBag.TotalWeeks = totalWeeks;

        return View();
    }

    public IActionResult WeekData(DateTime start, DateTime end)
    {
        int? userId = GetCurrentStudentID();

        if (userId == null)
        {
            return Unauthorized();
        }

        var rangeEnd = end.AddDays(1);

        var allMoods = _context.Moods
            .Where(m => m.StudentID == userId.Value
                     && m.EntryDate >= start
                     && m.EntryDate < rangeEnd)
            .Select(m => new { m.EntryDate, m.MoodValue })
            .ToList();

        var allSleeps = _context.Sleeps
            .Where(s => s.StudentID == userId.Value
                     && s.EntryDate >= start
                     && s.EntryDate < rangeEnd)
            .Select(s => new { s.EntryDate, s.HoursSlept, s.SleepQuality })
            .ToList();

        var days = Enumerable.Range(0, (end.Date - start.Date).Days + 1)
            .Select(i => start.Date.AddDays(i))
            .ToList();

        var mood = new List<double>();
        var sleep = new List<double>();
        var quality = new List<int>();

        foreach (var d in days)
        {
            var dayMoods = allMoods.Where(m => m.EntryDate.Date == d).ToList();
            var daySleeps = allSleeps.Where(s => s.EntryDate.Date == d).ToList();

            double m2 = dayMoods.Any()
                ? Math.Round(dayMoods.Average(m => (double)m.MoodValue), 2)
                : 0;

            double totalSleep = daySleeps.Any()
                ? Math.Round(daySleeps.Sum(s => s.HoursSlept), 2)
                : 0;

            int avgQuality = daySleeps.Any()
                ? (int)Math.Round(daySleeps.Average(s => (double)s.SleepQuality), 2)
                : 0;

            mood.Add(m2);
            sleep.Add(totalSleep);
            quality.Add(avgQuality);
        }

        return Json(new
        {
            dates = days.Select(d => d.ToString("yyyy-MM-dd")),
            mood,
            sleep,
            quality,
            avgMood = mood.Where(x => x > 0).DefaultIfEmpty(0).Average(),
            avgSleep = sleep.Where(x => x > 0).DefaultIfEmpty(0).Average(),
            avgQuality = quality.Where(x => x > 0).DefaultIfEmpty(0).Average()
        });
    }
}