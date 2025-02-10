using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Facial_Recognition.DAL.Models
{
    public class StudentCourse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; } // Student foreign key

        [Required]
        public int CourseId { get; set; } // Course foreign key

        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
