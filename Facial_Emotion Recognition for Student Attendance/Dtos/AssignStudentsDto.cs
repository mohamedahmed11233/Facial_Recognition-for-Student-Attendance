using LinkDev.Facial_Recognition.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Facial_Emotion_Recognition_for_Student_Attendance.Dtos
{
    public class AssignStudentsDto
    {
        public int CourseId { get; set; }
        public List<int> StudentIds { get; set; }
    }
}
