using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LinkDev.Facial_Recognition.DAL.Models;
using LinkDev.Facial_Recognition.DAL.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;

namespace LinkDev.Facial_Recognition.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(ApplicationDbContext context, ILogger<AttendanceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region 1️⃣ Record Attendance
        [HttpPost("record")]
        public async Task<IActionResult> StoreAIData([FromBody] List<Dictionary<string, string>> aiData)
        {
            if (aiData == null || aiData.Count < 2)
            {
                return BadRequest(new ApiResponse(400, "Invalid data format."));
            }

            // Extract student name, course name, lecture name, and attendance time
            string studentName = aiData[0].Keys.First();
            string attendanceTimeStr = aiData[0][studentName];
            string courseName = aiData[1].Keys.First();
            string lectureName = aiData[1][courseName];

            if (!DateTime.TryParse(attendanceTimeStr, out DateTime attendanceTime))
            {
                return BadRequest(new ApiResponse(400, "Invalid date format."));
            }

            // Find or create student
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Name == studentName);
            if (student == null)
            {
                student = new Student { Name = studentName };
                _context.Students.Add(student);
                await _context.SaveChangesAsync(); // Save immediately to generate an ID
            }

            // Find or create course
            var course = await _context.Courses.Include(c => c.Lectures)
                .FirstOrDefaultAsync(c => c.Name == courseName);
            if (course == null)
            {
                course = new Course { Name = courseName, Lectures = new List<Lecture>() };
                _context.Courses.Add(course);
                await _context.SaveChangesAsync(); // Save immediately to generate an ID
            }

            // Find or create lecture
            var lecture = course.Lectures.FirstOrDefault(l => l.Name == lectureName);
            if (lecture == null)
            {
                lecture = new Lecture { Name = lectureName, CourseId = course.Id };
                _context.Lectures.Add(lecture);
                await _context.SaveChangesAsync(); // Save immediately to generate an ID
            }

            // Store attendance record
            var attendance = new Attendance
            {
                UserId = student.Id,
                AttendanceTime = attendanceTime,
                LectureId = lecture.Id
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Attendance recorded successfully."));
        }

        #endregion

        #region 2️⃣ Helper Methods

        private DateTime? ValidateAndParseTimestamp(string timestamp)
        {
            if (DateTime.TryParse(timestamp, out DateTime parsedTime))
                return parsedTime;
            return null;
        }

        private async Task<Student> GetOrCreateStudentAsync(string studentName)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Name.ToLower() == studentName.ToLower());

            if (student == null)
            {
                student = new Student { Name = studentName };
                _context.Students.Add(student);
                await _context.SaveChangesAsync(); // Save student to database and get Id
                _logger.LogInformation($"Created new student: {studentName}");
            }

            return student;
        }

        private async Task<Lecture> GetOrCreateLectureAsync(string courseName, string lectureName)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Name.ToLower() == courseName.ToLower());

            if (course == null)
            {
                course = new Course { Name = courseName };
                _context.Courses.Add(course);
                await _context.SaveChangesAsync(); // Save course to database and get Id
                _logger.LogInformation($"Created new course: {courseName}");
            }

            var lecture = await _context.Lectures
                .FirstOrDefaultAsync(l => l.Name.ToLower() == lectureName.ToLower() && l.CourseId == course.Id);

            if (lecture == null)
            {
                lecture = new Lecture
                {
                    Name = lectureName,
                    CourseId = course.Id
                };
                _context.Lectures.Add(lecture);
                await _context.SaveChangesAsync(); // Save lecture to database and get Id
                _logger.LogInformation($"Created new lecture: {lectureName} for course {courseName}");
            }

            return lecture;
        }

        private async Task<bool> AttendanceExists(int studentId, int lectureId, DateTime attendanceTime)
        {
            return await _context.Attendances
                .AnyAsync(a => a.UserId == studentId && a.LectureId == lectureId && a.AttendanceTime == attendanceTime);
        }

        private async Task AddAttendanceAsync(Student student, Lecture lecture, DateTime attendanceTime)
        {
            var attendance = new Attendance
            {
                UserId = student.Id,
                AttendanceTime = attendanceTime,
                LectureId = lecture.Id
            };
            _context.Attendances.Add(attendance);
            _logger.LogInformation($"Added attendance for student {student.Name} in lecture {lecture.Name}.");
        }
        #endregion

        #region 3️⃣ Get Attendance for Course and Lecture
        [HttpGet("course/{courseName}/lecture/{lectureName}/attendance")]
        public async Task<IActionResult> GetAttendanceForCourseAndLecture(string courseName, string lectureName)
        {
            var attendanceRecords = await _context.Attendances
                .Where(a => a.Lecture.Course.Name.ToLower() == courseName.ToLower() && a.Lecture.Name.ToLower() == lectureName.ToLower())
                .Include(a => a.Student)
                .Include(a => a.Lecture)
                .ThenInclude(l => l.Course)
                .ToListAsync();

            if (!attendanceRecords.Any())
                return NotFound(new { Message = "No attendance records found for this course and lecture." });

            var result = attendanceRecords.Select(a => new
            {
                StudentName = a.Student.Name,
                AttendanceTime = a.AttendanceTime,
                CourseName = a.Lecture.Course.Name,
                LectureName = a.Lecture.Name
            });

            return Ok(result);
        }
        #endregion

        #region 2️⃣ Get All Attendance
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAttendance()
        {
            try
            {
                var attendanceRecords = await _context.Attendances
                    .Include(a => a.Student)
                    .Include(a => a.Lecture)
                        .ThenInclude(l => l.Course)
                    .Select(a => new
                    {
                        StudentId = a.Student.Id,
                        StudentName = a.Student.Name,
                        CourseName = a.Lecture.Course.Name,
                        LectureName = a.Lecture.Name,
                        AttendanceTime = a.AttendanceTime
                    })
                    .ToListAsync();

                if (!attendanceRecords.Any())
                    return NotFound(new { Message = "No attendance records found." });

                return Ok(attendanceRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching attendance records: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while fetching attendance records." });
            }
        }
        #endregion
    }
}
