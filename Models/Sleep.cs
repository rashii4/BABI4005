using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("sleep")]
public class Sleep
{
    [Key]
    public int SleepID { get; set; }

    public int StudentID { get; set; }

    [JsonIgnore]
    public Student? Student { get; set; }

    public double HoursSlept { get; set; }

    public int SleepQuality { get; set; }

    public string? Notes { get; set; }

    public DateTime EntryDate { get; set; } = DateTime.Now;
}