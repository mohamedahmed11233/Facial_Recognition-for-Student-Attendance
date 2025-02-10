using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkDev.Facial_Recognition.DAL.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign key to Student

        // Navigation property to Student
        public Student Student { get; set; }

        [Required]
        public DateTime AttendanceTime { get; set; }

        [Required]
        public int LectureId { get; set; } // Foreign key to Lecture

        // Navigation property to Lecture
        public Lecture Lecture { get; set; }
    }
}
