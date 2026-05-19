using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    // GET (load page)
    public IActionResult Index()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        return View();
    }

    //POST (handle login)
    [HttpPost]
    public IActionResult Index(string email, string password)
    {
        string error = "";

        if (!IsValidEmail(email))
        {
            error = "Invalid email format";
        }
        else
        {
            var user = _context.Students.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                error = "Email not found";
            }
            else if (user.Password != Hash(password))
            {
                error = "Invalid password";
            }
            else
            {
                // store session like PHP
                HttpContext.Session.SetString("FirstName", user.Firstname);
                HttpContext.Session.SetString("LastName", user.Lastname);

                return RedirectToAction("Index");
            }
        }

        ViewBag.Error = error;
        return View();
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private string Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}