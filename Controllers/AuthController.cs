using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    // 🔑 LOGIN
    public IActionResult Login()
    {
        return Redirect("/Identity/Account/Login");
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Students.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            ViewBag.Error = "Email not found";
            return View();
        }

        if (user.Password != Hash(password))
        {
            ViewBag.Error = "Invalid password";
            return View();
        }

        HttpContext.Session.SetString("user", user.Firstname + " " + user.Lastname);
        HttpContext.Session.SetString("StudentID", user.StudentID.ToString());
        HttpContext.Session.SetString("FirstName", user.Firstname);
        HttpContext.Session.SetString("LastName", user.Lastname);
        return RedirectToAction("Index", "Home");
    }

    // 📝 REGISTER
    public IActionResult Register()
    {
        return Redirect("/Identity/Account/Register");
    }

    [HttpPost]
    public IActionResult Register(Student model)
    {
        if (_context.Students.Any(u => u.Email == model.Email))
        {
            ViewBag.Error = "Email already exists";
            return View();
        }

        model.Password = Hash(model.Password);

        _context.Students.Add(model);
        _context.SaveChanges();

        return RedirectToAction("Login", "Auth");
    }

    // 🔓 LOGOUT
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Redirect("/Identity/Account/Logout");
    }

    // 🔐 HASH FUNCTION
    private string Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}