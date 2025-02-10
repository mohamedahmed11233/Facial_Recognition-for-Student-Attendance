using LinkDev.Facial_Recognition.DAL.Models;
using System.ComponentModel.DataAnnotations;

public class Emotion
{
    [Key]
    public int Id { get; set; }


    [Required]
    public string EmotionDetails { get; set; } // e.g., {"Happy": 3, "Sad": 1}


    public int? LectureId { get; set; } // Make it nullable

    public Lecture? Lecture { get; set; } // Optional navigation property
}
