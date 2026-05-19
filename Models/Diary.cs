using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("diary")]
public class Diary
{
    [Key]
    public int EntryID { get; set; }

    public int StudentID { get; set; }

    [JsonIgnore]
    public Student? Student { get; set; }

    public string Title { get; set; } = "";

    public string Content { get; set; } = "";

    public DateTime EntryDate { get; set; } = DateTime.Now;
}