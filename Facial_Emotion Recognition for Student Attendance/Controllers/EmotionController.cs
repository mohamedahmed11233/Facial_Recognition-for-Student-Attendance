using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using LinkDev.Facial_Recognition.DAL.Data;
using LinkDev.Facial_Recognition.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LinkDev.Facial_Recognition.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmotionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmotionController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("emotion")]
        public async Task<IActionResult> StoreAIDataWithEmotion([FromBody] List<Dictionary<string, string>> aiData)
        {
            if (aiData == null || aiData.Count < 2)
            {
                return BadRequest(new ApiResponse(400, "Invalid data format."));
            }

            // Extract emotion details
            var emotionData = aiData[0]; // Emotions data (Angry, Happy, etc.)
            var courseAndLecture = aiData[1];

            // Ensure course and lecture data are valid
            if (courseAndLecture.Count != 1)
            {
                return BadRequest(new ApiResponse(400, "Course and lecture data format is invalid."));
            }

            string courseName = courseAndLecture.FirstOrDefault().Key; // Course Name
            string lectureName = courseAndLecture.FirstOrDefault().Value; // Lecture Name

            // Check if required emotion data is present
            if (!emotionData.ContainsKey("Angry") || !emotionData.ContainsKey("Happy") || !emotionData.ContainsKey("Neutral"))
            {
                return BadRequest(new ApiResponse(400, "Emotion data is incomplete."));
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

            // Store emotion record
            var emotion = new Emotion
            {
                EmotionDetails = System.Text.Json.JsonSerializer.Serialize(emotionData),
                LectureId = lecture.Id
            };

            _context.Emotions.Add(emotion);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Emotion data recorded successfully."));
        }

        [HttpGet("all-emotions")]
        public async Task<IActionResult> GetAllEmotions()
        {
            try
            {
                // Retrieve all emotions with related lecture and course data
                var emotions = await _context.Emotions
                    .Include(e => e.Lecture)
                    .ThenInclude(l => l.Course)
                    .Select(e => new
                    {
                        e.Id,
                        e.EmotionDetails,
                        LectureName = e.Lecture.Name,
                        CourseName = e.Lecture.Course.Name,
                    })
                    .ToListAsync();

                // Return the list of emotions
                return Ok(emotions);
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is configured)
                // Handle any potential errors
                return StatusCode(500, new ApiResponse(500, "An error occurred while retrieving emotions.", ex.Message));
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchEmotions([FromQuery] string? courseName, [FromQuery] string? lectureName)
        {
            try
            {
                // Query the emotions table with related lecture and course data
                var query = _context.Emotions
                    .Include(e => e.Lecture)
                    .ThenInclude(l => l.Course)
                    .AsQueryable();

                // Filter by course name if provided
                if (!string.IsNullOrEmpty(courseName))
                {
                    query = query.Where(e => e.Lecture.Course.Name.Contains(courseName));
                }

                // Filter by lecture name if provided
                if (!string.IsNullOrEmpty(lectureName))
                {
                    query = query.Where(e => e.Lecture.Name.Contains(lectureName));
                }

                // Fetch the results
                var emotions = await query
                    .Select(e => new
                    {
                        e.Id,
                        e.EmotionDetails,
                        LectureName = e.Lecture.Name,
                        CourseName = e.Lecture.Course.Name,
                    })
                    .ToListAsync();

                // Return filtered results
                return Ok(emotions);
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is configured)
                // Handle any potential errors
                return StatusCode(500, new ApiResponse(500, "An error occurred while searching for emotions."));
            }
        }


    }
    public class EmotionDto
    {
        public string UserId { get; set; }
        public DateTime DetectionTime { get; set; }
        public Dictionary<string, int> Emotions { get; set; }
        public int LectureId { get; set; }
    }

}
