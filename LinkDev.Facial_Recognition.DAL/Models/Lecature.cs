using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Facial_Recognition.DAL.Models
{
    public class Lecture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // اسم المحاضرة

        [Required]
        public int CourseId { get; set; } // Foreign key to Course

        // Navigation property to Course
        public Course Course { get; set; }

        // List of attendance records for this lecture
        public ICollection<Attendance> Attendances { get; set; }
        public ICollection<Emotion> Emotions { get; set; }

    }
}
