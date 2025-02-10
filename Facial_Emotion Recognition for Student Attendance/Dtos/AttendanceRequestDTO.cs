namespace Facial_Emotion_Recognition_for_Student_Attendance.Dtos
{

    public class AttendanceRequestDTO
    {
        public int UserId { get; set; } // Foreign key to IdentityUser
        public string ImageBase64 { get; set; } // Base64-encoded image used for recognition
        public string Emotion { get; set; } // Optional: Detected emotion

    }
}
