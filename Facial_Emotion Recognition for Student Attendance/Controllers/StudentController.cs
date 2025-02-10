//using Facial_Emotion_Recognition_for_Student_Attendance.Dtos;
//using LinkDev.Facial_Recognition.DAL.Data;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using LinkDev.Facial_Recognition.BLL.Helper.Errors;

//namespace Facial_Emotion_Recognition_for_Student_Attendance.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class StudentController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public StudentController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpDelete("remove-student")]
//        public async Task<IActionResult> RemoveStudentFromCourse([FromBody] AssignStudentsDto model)
//        {
//            if (model.CourseId <= 0 || model.StudentIds == null || !model.StudentIds.Any())
//            {
//                return BadRequest(new ApiResponse(400, "Invalid request."));
//            }

//            var course = await _context.Courses.Include(c => c.StudentCourses)
//                                               .FirstOrDefaultAsync(c => c.Id == model.CourseId);

//            if (course == null)
//            {
//                return NotFound(new ApiResponse(404, "Course not found."));
//            }

//            var studentsToRemove = course.StudentCourses.Where(sc => model.StudentIds.Contains(sc.StudentId)).ToList();

//            if (!studentsToRemove.Any())
//            {
//                return NotFound(new ApiResponse(404, "No matching students found in this course."));
//            }

//            _context.StudentCourses.RemoveRange(studentsToRemove);
//            await _context.SaveChangesAsync();

//            return Ok(new ApiResponse(200, "Students removed from the course successfully.", studentsToRemove));
//        }
//    }
//}
