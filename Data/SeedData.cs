using System.Security.Cryptography;
using System.Text;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        // Make sure database exists
        context.Database.EnsureCreated();

        // If seed data already exists, do not add it again
        if (context.Students.Any(s => s.Email == "test1@bcit.ca"))
        {
            return;
        }

        var student1 = new Student
        {
            Email = "test1@bcit.ca",
            Password = Hash("123"),
            Firstname = "Test",
            Lastname = "One",
            Institution = "BCIT"
        };

        var student2 = new Student
        {
            Email = "test2@bcit.ca",
            Password = Hash("123"),
            Firstname = "Test",
            Lastname = "Two",
            Institution = "BCIT"
        };

        context.Students.AddRange(student1, student2);
        context.SaveChanges();

        context.Moods.AddRange(
            new Mood
            {
                StudentID = student1.StudentID,
                MoodValue = 4,
                Notes = "Feeling good today",
                EntryDate = DateTime.Now.AddDays(-2)
            },
            new Mood
            {
                StudentID = student1.StudentID,
                MoodValue = 3,
                Notes = "Average day",
                EntryDate = DateTime.Now.AddDays(-1)
            }
        );

        context.Sleeps.AddRange(
            new Sleep
            {
                StudentID = student1.StudentID,
                HoursSlept = 7.5,
                SleepQuality = 3,
                Notes = "Slept okay",
                EntryDate = DateTime.Now.AddDays(-2)
            },
            new Sleep
            {
                StudentID = student1.StudentID,
                HoursSlept = 6.5,
                SleepQuality = 2,
                Notes = "Could have slept more",
                EntryDate = DateTime.Now.AddDays(-1)
            }
        );

        context.Diaries.AddRange(
            new Diary
            {
                StudentID = student1.StudentID,
                Title = "First Reflection",
                Content = "Today I used Restora to track my wellness.",
                EntryDate = DateTime.Now.AddDays(-2)
            },
            new Diary
            {
                StudentID = student1.StudentID,
                Title = "Second Reflection",
                Content = "I noticed how sleep affects my mood.",
                EntryDate = DateTime.Now.AddDays(-1)
            }
        );

        context.SaveChanges();
    }

    private static string Hash(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}