    using Facial_Emotion_Recognition_for_Student_Attendance.Dtos;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using LinkDev.Facial_Recognition.BLL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Facial_Emotion_Recognition_for_Student_Attendance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAuthService _authService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new ApiResponse(401, "Invalid Email or Password"));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401, "Invalid Email or Password"));

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new UserDto
            {
                Token = await _authService.CreateTokenAsync(user.Email, user.UserName, roles),
                Email = user.Email,      // Add Email here
                DisplayName = user.UserName     // Add UserName here
            });
        }

        // Register method accepting JSON data
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterDto model)
        {
            var user = new IdentityUser
            {
                UserName = model.Email.Split("@")[0],
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "User registration failed"));

            return Ok(new ApiResponse(200, "User registered successfully."));
        }
    }
}
