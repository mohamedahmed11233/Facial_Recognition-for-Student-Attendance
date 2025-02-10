namespace LinkDev.Facial_Recognition.DAL.DTOs
{
    public class AttendanceResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string StudentName { get; set; } // Student's name
        public DateTime AttendanceTime { get; set; }
        public string Emotion { get; set; } // Optional: Detected emotion
    }
}