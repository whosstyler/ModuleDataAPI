using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/oauth")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserService _userService;

        public AuthController(TokenService tokenService, UserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        // Register Endpoint
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.HWID))
            {
                return BadRequest(new { Message = "Username, password, and HWID are required." });
            }

            if (_userService.UserExists(request.Username))
            {
                return Conflict(new { Message = "User already exists." });
            }

            var hashedPassword = _userService.HashPassword(request.Password);
            var user = new User
            {
                Username = request.Username,
                Password = hashedPassword,
                HWID = request.HWID,
                Role = Role.User,
                Avatar = request.Avatar // Optional
            };

            _userService.CreateUser(user);

            return Ok(new { Message = "User registered successfully." });
        }

        // Login Endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.HWID))
            {
                return BadRequest(new { Message = "Username, password, and HWID are required." });
            }

            var user = _userService.GetUser(request.Username);
            if (user == null || !_userService.VerifyPassword(request.Password, user.Password) || user.HWID != request.HWID)
            {
                return Unauthorized(new { Message = "Invalid username, password, or HWID." });
            }

            // Generate token valid for 1 hour
            var token = _tokenService.GenerateToken(user);
            user.Token = token;

            // Update user token and expiration time
            _userService.UpdateUserToken(user);

            return Ok(new { Token = token });
        }

        // Token Validation Endpoint
        [HttpPost("token")]
        public IActionResult AuthByToken([FromBody] TokenRequest request)
        {
            var token = request.Token;
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Authorized = false, Message = "Token is missing" });
            }

            var isAuthorized = _tokenService.ValidateToken(token);

            return Ok(new { Authorized = isAuthorized });
        }
    }
}
