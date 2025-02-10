using Facial_Emotion_Recognition_for_Student_Attendance.Dtos;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using LinkDev.Facial_Recognition.DAL.Data;
using LinkDev.Facial_Recognition.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facial_Emotion_Recognition_for_Student_Attendance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new course
        [HttpPost("create")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest(new ApiResponse(400, "Invalid course name."));
            }

            var course = new Course
            {
                Name = model.Name,
                StudentCourses = new List<StudentCourse>()
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse(200, $"Course created successfully. CourseId: {course.Id}, CourseName: {course.Name}"));
        }

        // Get all courses
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _context.Courses
                .Select(course => new
                {
                    CourseId = course.Id,
                    CourseName = course.Name
                })
                .ToListAsync();

            if (!courses.Any())
            {
                return NotFound(new ApiResponse(404, "No courses found."));
            }

            return Ok(courses);
        }

        // Assign students to a course
        [HttpPost("assign-students")]
        public async Task<IActionResult> AssignStudentsToCourse([FromBody] AssignStudentsDto model)
        {
            if (model.StudentIds == null || !model.StudentIds.Any() || model.CourseId <= 0)
            {
                return BadRequest(new ApiResponse(400, "Invalid student or course data."));
            }

            var course = await _context.Courses
                                       .Include(c => c.StudentCourses)
                                       .FirstOrDefaultAsync(c => c.Id == model.CourseId);

            if (course == null)
            {
                return NotFound(new ApiResponse(404, "Course not found."));
            }

            var existingStudentIds = course.StudentCourses.Select(sc => sc.StudentId).ToHashSet();
            var newStudentIds = model.StudentIds.Where(id => !existingStudentIds.Contains(id)).ToList();

            if (!newStudentIds.Any())
            {
                return Ok(new ApiResponse(200, "All students are already assigned to the course."));
            }

            foreach (var studentId in newStudentIds)
            {
                _context.StudentCourses.Add(new StudentCourse
                {
                    StudentId = studentId,
                    CourseId = model.CourseId
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse(200, "Students assigned to course successfully."));
        }

        // Get all students assigned to a course
        [HttpGet("{courseId}/students")]
        public async Task<IActionResult> GetCourseStudents(int courseId)
        {
            var students = await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Include(sc => sc.Student)
                .Select(sc => new
                {
                    Id = sc.Student.Id,
                    Name = sc.Student.Name
                })
                .ToListAsync();

            if (!students.Any())
            {
                return NotFound(new ApiResponse(404, "No students found for this course."));
            }

            return Ok(students);
        }

        // Remove students from a course
        [HttpDelete("remove-students")]
        public async Task<IActionResult> RemoveStudentsFromCourse([FromBody] AssignStudentsDto model)
        {
            if (model.StudentIds == null || !model.StudentIds.Any() || model.CourseId <= 0)
            {
                return BadRequest(new ApiResponse(400, "Invalid student or course data."));
            }

            var course = await _context.Courses
                                       .Include(c => c.StudentCourses)
                                       .FirstOrDefaultAsync(c => c.Id == model.CourseId);

            if (course == null)
            {
                return NotFound(new ApiResponse(404, "Course not found."));
            }

            var studentsToRemove = course.StudentCourses.Where(sc => model.StudentIds.Contains(sc.StudentId)).ToList();

            if (!studentsToRemove.Any())
            {
                return Ok(new ApiResponse(200, "No students found to remove from this course."));
            }

            _context.StudentCourses.RemoveRange(studentsToRemove);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Students removed from course successfully."));
        }

        // Delete a course
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var course = await _context.Courses
                                       .Include(c => c.StudentCourses)
                                       .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound(new ApiResponse(404, "Course not found."));
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Course deleted successfully."));
        }
    }
    public class CreateCourseDto
    {
        public string Name { get; set; }
    }
}
