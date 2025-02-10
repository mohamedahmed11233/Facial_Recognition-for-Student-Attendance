
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace LinkDev.Facial_Recognition.DAL.Models
    {
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // اسم الكورس

        // قائمة الطلاب الذين يتبعون هذا الكورس
        public List<StudentCourse> StudentCourses { get; set; }

        // قائمة المحاضرات التابعة لهذا الكورس
        public ICollection<Lecture> Lectures { get; set; }
    }
}
