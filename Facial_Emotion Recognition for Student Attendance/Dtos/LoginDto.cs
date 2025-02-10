using System.ComponentModel.DataAnnotations;

namespace Facial_Emotion_Recognition_for_Student_Attendance.Dtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
