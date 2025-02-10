namespace Facial_Emotion_Recognition_for_Student_Attendance.Dtos
{
    public class EmotionResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime DetectionTime { get; set; }
        public string EmotionType { get; set; }
    }
}