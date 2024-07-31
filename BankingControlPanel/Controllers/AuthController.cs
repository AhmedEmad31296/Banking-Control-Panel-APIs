using BankingControlPanel.Application.Services.Users;
using BankingControlPanel.Application.Services.Users.Dto;
using BankingControlPanel.Authentication;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingControlPanel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtAuthentication _jwtAuthentication;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IUserService userService, IJwtAuthentication jwtAuthentication, ILogger<AuthController> logger)
        {
            _userService = userService;
            _jwtAuthentication = jwtAuthentication;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(model, model.Role);

                if (result.Succeeded)
                {
                    return Ok(new { result = "User created successfully!" });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user.");
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.Succeeded)
                {
                   var userRoles = await _userService.GetUserRolesAsync(model.Email);
                    // Generate JWT token
                    var token = _jwtAuthentication.GenerateToken(model.Email, userRoles);
                    return Ok(new { token });
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

    }
}
