using LinkDev.Facial_Recognition.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkDev.Facial_Recognition.DAL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Emotion> Emotions { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Lecture> Lectures { get; set; } // Added Lectures DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إعداد العلاقة بين الحضور والطلاب
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Lecture)
                .WithMany(l => l.Attendances)
                .HasForeignKey(a => a.LectureId)
                .OnDelete(DeleteBehavior.Cascade);

            // إعداد العلاقة بين الطلاب والكورسات (Many-to-Many)
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            // إعداد العلاقة بين الكورسات والمحاضرات
            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lectures)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
