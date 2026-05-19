using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("mood")]
public class Mood
{
    [Key]
    public int MoodID { get; set; }

    public int StudentID { get; set; }

    [JsonIgnore]
    public Student? Student { get; set; }

    public int MoodValue { get; set; }

    public string? Notes { get; set; }

    public DateTime EntryDate { get; set; } = DateTime.Now;
}