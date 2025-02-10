using System.ComponentModel.DataAnnotations;

namespace Facial_Emotion_Recognition_for_Student_Attendance.Dtos
{
    public class RegisterDto
    {
           [Required(ErrorMessage = "Email is required")]
           [EmailAddress]
           public string Email { get; set; }
           [Required(ErrorMessage = "DisplayName is required")]
           public string DisplayName { get; set; }
           [Required(ErrorMessage = "PhoneNumber is required")]
           [Phone]
           public string PhoneNumber { get; set; }
           [Required(ErrorMessage = "Password is required")]
           public string Password { get; set; }
        
    }
}
