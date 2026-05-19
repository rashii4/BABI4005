using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Student> Students { get; set; }

    public DbSet<Diary> Diaries { get; set; }

    public DbSet<Mood> Moods { get; set; }

    public DbSet<Sleep> Sleeps { get; set; }
}