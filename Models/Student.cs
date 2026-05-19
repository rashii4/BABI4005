using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Student
{
    [Key]
    public int StudentID { get; set; }

    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    public string Firstname { get; set; } = "";
    public string Lastname { get; set; } = "";
    public string Institution { get; set; } = "";

    [JsonIgnore]
    public ICollection<Mood> Moods { get; set; } = new List<Mood>();

    [JsonIgnore]
    public ICollection<Sleep> Sleeps { get; set; } = new List<Sleep>();

    [JsonIgnore]
    public ICollection<Diary> Diaries { get; set; } = new List<Diary>();
}