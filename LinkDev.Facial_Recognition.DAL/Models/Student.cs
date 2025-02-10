using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Facial_Recognition.DAL.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; } // Student ID (can be the same as UserId)

        [Required]
        public string Name { get; set; } // Student's name

        // Navigation property for Student-Course many-to-many relationship
        public ICollection<StudentCourse> StudentCourses { get; set; }

        // Navigation property for attendance records
        public ICollection<Attendance> Attendances { get; set; }
    }
}
